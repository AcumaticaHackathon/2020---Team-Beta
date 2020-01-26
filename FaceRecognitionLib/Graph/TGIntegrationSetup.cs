using System;
using System.Collections;
using System.Linq;
using System.Threading;
using face_quickstart;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using PX.Common;
using PX.Data;
using PX.Objects.EP;

namespace TGIntegration
{
	public class TGIntegrationSetup : PXGraph<TGIntegrationSetup>
	{
		public PXSave<TGSetup> Save;
		public PXCancel<TGSetup> Cancel;

		public PXAction<TGSetup> TrainFaceRecongnition;
		[PXButton()]
		[PXUIField(DisplayName = "Train Face Images", Visible = true)]
		public virtual IEnumerable trainFaceRecongnition(PXAdapter adapter)
		{
			Save.Press();

			PXLongOperation.StartOperation(this, delegate ()
			{
				IFaceClient client = Trainer.Authenticate(MasterView.Current.FaceApiEndpoint, MasterView.Current.FaceApiSubscriptionKey);
				var trainer = new Trainer(client, MasterView.Current.FaceApiGroupID);
				bool uploaded = false;
				foreach (EPEmployee emp in PXSelect<EPEmployee>.Select(this))
				{
					bool deleted = false;
					Person person = null;
					Guid[] files = PXNoteAttribute.GetFileNotes(this.Caches[typeof(EPEmployee)], emp);

					foreach (Guid fileID in files)
					{
						var fm = new PX.SM.UploadFileMaintenance();
						PX.SM.FileInfo fi = fm.GetFile(fileID);
						string ext = System.IO.Path.GetExtension(fi.Name).ToLower();
						PXTrace.WriteWarning(emp.AcctCD.TrimEnd() + " " + ext);
						if (ext.IsIn(".png", ".jpg", ".bmp"))
						{
							if (!deleted)
							{
								var utilities = new Utilities(client, MasterView.Current.FaceApiGroupID);
								utilities.DeleteEmployee(emp.AcctCD.TrimEnd());
								deleted = true;
							}

							if (person == null)
							{
								person = trainer.GetEmployee_CreateIfNonExistent(emp.AcctCD.TrimEnd());
							}


							using (var stream = new System.IO.MemoryStream(fi.BinData))
							{
								PXTrace.WriteWarning(emp.AcctCD.TrimEnd() + " " + fi.Name + " " + stream.Length);
								trainer.UploadTrainingImageStream(person, stream);
								Thread.Sleep(1000);
								uploaded = true;
							}
						}

					}
				}
				if (uploaded)
				{
					trainer.InvokeTraining();
				}
			});


			return adapter.Get();
		}

		public PXSelect<TGSetup> MasterView;

	}
}
