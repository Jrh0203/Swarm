using System;

namespace BehaviorTreeSpace
{
    public abstract class Gadget : Behavior 
    {
        protected Behavior child;
        public Gadget()
        {
            Initialize = () => { };
            Terminate = status => { };
            UpdateAct = () => Status.INVALID;
        }
        public void SetChild(Behavior c) { child = c; }
    }
	public class InverterGadget : Gadget
    {
        public InverterGadget()
        {
            UpdateAct = () =>
            {
                Status s = child.UpdateAct();
                if (s == Status.SUCCESS)
                {
                    return Status.FAILURE;
                }
                else if (s == Status.FAILURE)
                {
                    return Status.SUCCESS;
                }
                return s;
            };
        }
    }
    public class LoopGadget : Gadget
    {
        int loopNum;
        int count;
        public LoopGadget(int n)
        {
            loopNum = n;
            count = 0;
            UpdateAct = () =>
            {
                Status s = child.Update();

                if (count >= loopNum) return Status.SUCCESS;
                if (s == Status.FAILURE || s == Status.INVALID) return s;
                if (s == Status.SUCCESS) count++;
                return Status.RUNNING;
            };
        }
    }
}
