using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{
    class Job
    {
        public Job(int computerId, string command, int processId, int returnValue, DateTime startTime)
        {
            ComputerId = computerId;
            Command = command;
            ProcessId = processId;
            ReturnValue = returnValue;
            StartTime = startTime;
            State = States.Started;
        }
        public enum States
        {
            Started,
            Finished,
            Unknown,
            Broken
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ComputerId { get; set; }
        public string Command { get; set; }
        public int ProcessId { get; set; }
        public int ReturnValue { get; set; }
        public DateTime StartTime { get; set; }
        public States State { get; set; }
    }
}
