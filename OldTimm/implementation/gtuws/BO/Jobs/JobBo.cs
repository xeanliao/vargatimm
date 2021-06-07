using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GTU.ModelLayer.Jobs;
using GTU.ModelLayer.Common;
using System.IO;

namespace GTU.BusinessLayer.Jobs
{
    public class JobBo
    {
        public JobBo()
        {
        }

        public Job ReadJob()
        {
            Job job = new Job();
            // Constructs an instance of the XmlSerializer with the type of object that is being deserialized.
            XmlSerializer mySerializer = new XmlSerializer(typeof(Job));

            // To read the file, creates a FileStream.  
            FileStream myFileStream = new FileStream("job1.xml", FileMode.Open);

            // Calls the Deserialize method and casts to the object type.  
            job = (Job)mySerializer.Deserialize(myFileStream);

            return job;
        }
    }
}
