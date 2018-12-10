using System;
using UnityEngine;

namespace BehaviorTreeSpace
{
    public class Condition : Behavior
    {
        public Condition(Func<bool> f)
        {
            Initialize = () =>
            {

            };
            UpdateAct = () =>
            {
                if(f())
                    return Status.SUCCESS;
                return Status.FAILURE;
            };
            Terminate = status =>
            {

            };
        }
    }

    public class BAction : Behavior
    {
        public BAction(Func<Status> a)
        {
            Initialize = () =>
            {
 
            };
            UpdateAct = () =>
            {
                return a();
            };
            Terminate = status =>
            {
                
            };
        }
    }
}