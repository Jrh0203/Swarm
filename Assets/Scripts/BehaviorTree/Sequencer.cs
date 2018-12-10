using System;

namespace BehaviorTreeSpace
{
    public class Sequencer : Composite
    {
        int index = 0;
        public Sequencer()
        {

            UpdateAct = () =>
            {

                Status s = GetChild(index).Update();
                if (s == Status.INVALID) return Status.INVALID;


                if (s == Status.FAILURE)
                {
                    index = 0;
                    return Status.FAILURE;
                }
                
                if (s == Status.SUCCESS)
                {
                    if (++index == ChildCount)
                    {
                        index = 0;
                        return Status.SUCCESS;
                    }
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