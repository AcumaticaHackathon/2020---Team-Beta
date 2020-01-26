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

	[Serializable]
	[PXCacheName("Toggle Integration Setup")]
	public class TGSetup : IBqlTable
	{
		#region FaceApiSubscriptionKey

		public abstract class faceApiSubscriptionKey : IBqlField { }
		[PXDBString(50)]
		[PXDefault]
		[PXUIField(DisplayName = "Face API Subscription Key")]
		public virtual string FaceApiSubscriptionKey
		{ get; set; }

		#endregion
		#region FaceApiEndpoint

		public abstract class faceApiEndpoint : IBqlField { }
		[PXDBString(50)]
		[PXDefault]
		[PXUIField(DisplayName = "Face API Endpoint")]
		public virtual string FaceApiEndpoint
		{ get; set; }

		#endregion
		#region FaceApiGroupID

		public abstract class faceApiGroupID : IBqlField { }
		[PXDBString(36)]
		[PXDefault]
		[PXUIField(DisplayName = "Face API Group ID")]
		public virtual string FaceApiGroupID
		{ get; set; }

		#endregion

		#region FaceApiConfidenceThreshold

		public abstract class faceApiConfidenceThreshold : IBqlField { }
		[PXDBDecimal(6, MinValue = 0, MaxValue = 1)]
		[PXDefault]
		[PXUIField(DisplayName = "Face API Confidence Threshold")]
		public virtual decimal? FaceApiConfidenceThreshold
		{ get; set; }

		#endregion
		#region MapQuestApiKey

		public abstract class mapQuestApiKey : IBqlField { }
		[PXDBString(100)]
		[PXDefault]
		[PXUIField(DisplayName = "Map Quest API Key")]
		public virtual string MapQuestApiKey
		{ get; set; }

		#endregion

		#region tstamp

		public abstract class tstamp : IBqlField { }
		[PXDBTimestamp]
		public virtual byte[] Tstamp
		{ get; set; }

		#endregion
	}
}