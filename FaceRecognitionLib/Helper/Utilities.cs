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
	public class Utilities
	{

		public string PersonGroupID { get; set; }
		public IFaceClient Client { get; set; }

		public Utilities(IFaceClient client, string personGroupId)
		{
			this.Client = client;
			this.PersonGroupID = personGroupId;
		}


		public void DeleteEmployee(string EmployeeID)
		{
			var employee = GetEmployee(EmployeeID);
			if (employee == null) return;
			var result = Client.PersonGroupPerson.DeleteWithHttpMessagesAsync(PersonGroupID, employee.PersonId);
		}


		public Person GetEmployee(string employeeId)
		{
			var listOfPersons = Client.PersonGroupPerson.ListWithHttpMessagesAsync(PersonGroupID).Result;
			var Employee = listOfPersons.Body.SingleOrDefault(p => p.Name == employeeId);
			return Employee;
		}

	}
}
