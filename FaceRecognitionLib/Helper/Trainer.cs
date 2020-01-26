using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace face_quickstart
{
	public class Trainer
	{
		public static IFaceClient Authenticate(string endpoint, string key)
		{
			return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
		}


		private string PersonGroupID;

		public Trainer(IFaceClient client, string personGroupId)
		{
			this.Client = client;
			this.PersonGroupID = personGroupId;
		}

		public IFaceClient Client { get; set; }



		public void TrainDirectoryOfImages(string trainFaceDirectory, string employeeId)
		{
			//Client.PersonGroupPerson.ListAsync(PersonGroupID).Wait();
			//var test = Client.PersonGroupPerson.ListWithHttpMessagesAsync(PersonGroupID);
			//var test = Client.PersonGroup.GetAsync(PersonGroupID).Result;

			var employee = GetEmployee_CreateIfNonExistent(employeeId);
			var imageDirectory = new DirectoryInfo(trainFaceDirectory);
			foreach (var file in imageDirectory.GetFiles())
			{
				UploadTrainingImage(employee, file);
				Thread.Sleep(1000);
			}
			Console.WriteLine(employee.Name);
			//Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: groupedFace);
			InvokeTraining();
		}


		public void InvokeTraining()
		{
			// Start to train the person group.
			Console.WriteLine();
			Console.WriteLine($"Train person group {PersonGroupID}.");
			Client.PersonGroup.TrainAsync(PersonGroupID);

			// Wait until the training is completed.
			while (true)
			{
				//await Task.Delay(1000);
				Thread.Sleep(1000);
				var trainingStatus = Client.PersonGroup.GetTrainingStatusAsync(PersonGroupID).Result;
				Console.WriteLine($"Training status: {trainingStatus.Status}.");
				if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
			}

		}

		public void UploadTrainingImage(Person employee, FileInfo file)
		{
			Stream faceImage = file.OpenRead();

			var result = Client.PersonGroupPerson.AddFaceFromStreamAsync(PersonGroupID, employee.PersonId, faceImage, $"ImagePath:{file.FullName}").Result;
			Console.WriteLine(result.UserData);
		}

		public void UploadTrainingImageStream(Person employee, Stream file)
		{

			var result = Client.PersonGroupPerson.AddFaceFromStreamAsync(PersonGroupID, employee.PersonId, file).Result;
			Console.WriteLine(result.UserData);
		}


		public Person GetEmployee_CreateIfNonExistent(string employeeId)
		{
			var listOfPersons = Client.PersonGroupPerson.ListWithHttpMessagesAsync(PersonGroupID).Result;
			var Employee = listOfPersons.Body.SingleOrDefault(p => p.Name == employeeId);
			if (Employee != null) return Employee;

			var r = Client.PersonGroupPerson.CreateAsync(PersonGroupID, employeeId, "{OtherData:[Data1:'SomeValue1',data2:'SomeValue2']}").Result;
			//for some reason we get an object back with null name but the name 
			//is present if we re invoke the lookup.
			listOfPersons = Client.PersonGroupPerson.ListWithHttpMessagesAsync(PersonGroupID).Result;
			Employee = listOfPersons.Body.Single(p => p.Name == employeeId); //should raise error if it is still empty. 
			return Employee;
		}
	}
}

