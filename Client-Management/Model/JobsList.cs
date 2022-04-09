using System.Collections.Generic;

namespace Client_Management.Model
{
    class JobsList
    {
        static JobsList _jobs;
        public List<Job> JobList { get; set; }


        private JobsList()
        {
            JobList = new List<Job>();
        }

        static public JobsList GetInstance()
        {
            if (_jobs == null)
            {
                _jobs = new JobsList();
            }
            return _jobs;
        }
        static public void SetInstance(JobsList newJobs)
        {
            _jobs = newJobs;
        }
    }
}
