using System;
using System.Collections.Generic;


namespace BehaviorTreeSpace
{
	public abstract class Composite : Behavior
    {
        protected List<Behavior> Children { get; set; }
        protected Composite()
        {
            Children = new List<Behavior>();
            Initialize = () => { };
            Terminate = status => { };
            UpdateAct = () => Status.RUNNING;
        }
        public Behavior GetChild(int index)
        {
            return Children[index];
        }
        public int ChildCount
        {
            get { return Children.Count; }
        }
        public void AddChild(Behavior newChild)
        {
            Children.Add(newChild);
        }
    }
}
