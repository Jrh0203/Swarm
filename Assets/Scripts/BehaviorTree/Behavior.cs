using System;
using UnityEngine;

namespace BehaviorTreeSpace
{
    public enum Status { INVALID, SUCCESS, FAILURE, RUNNING };
    public class Behavior
    {
        public Status Status { get; set; } 
        public Action Initialize { get;  set; } 
        public Func<Status> UpdateAct { get;  set; }
        public Action<Status> Terminate { get; set; }

        // what should be called every tick
        public Status Update()
        {
            // check if this node has ben initialized, if not,  init
            if (Status == Status.INVALID && Initialize != null)
            {
                Initialize();
                Status = Status.RUNNING;
            }

            // call the update action, and set the status to that action
            Status = UpdateAct();

            // if the status isn't running, run terminate code
            if (Status != Status.RUNNING && Terminate != null)
            {
                Terminate(Status);
            }
            // return nodes status
            return Status;
        }
    }
}
