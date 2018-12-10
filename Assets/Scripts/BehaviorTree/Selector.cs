using System;
using UnityEngine;

namespace BehaviorTreeSpace
{
	public class Selector : Composite
    {
        int index = 0;
        public Selector()
        {
        
            UpdateAct = () =>
            {
   
                Status s = GetChild(index).Update();
                if (s == Status.INVALID) return Status.INVALID;

                if (s != Status.FAILURE)
                {
                    if (s == Status.SUCCESS)
                    {
                        index = 0;
                        return Status.SUCCESS;
                    }
                }
                else if (++index == ChildCount)
                {
                    index = 0;
                    return Status.FAILURE;
                }
                return Status.RUNNING;
               
            };
            Initialize = () =>
            {
                index = 0;
            };
        }
    }
}
