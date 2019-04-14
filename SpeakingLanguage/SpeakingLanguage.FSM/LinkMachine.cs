using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    public sealed class LinkMachine
    {
        private readonly int _endPoint;
        private readonly CellDictionary _cellDic = new CellDictionary();

        public LinkMachine(int endPoint)
        {
            _endPoint = endPoint;
        }

        public unsafe void ExecuteFrame()
        {
            var actorList = Component.SLComponent.Root.Find(Component.ComponentType.Actor);
            var actorIter = actorList.GetEnumerator();
            while (actorIter.MoveNext())
            {
                actorIter.Current.UnlinkTo(Component.ComponentType.Cell);

                var physicProp = actorIter.Current.Get<Component.Property.Physics>();


            }

            var behaviourList = Component.SLComponent.Root.Find(Component.ComponentType.Behaviour);
            var behaviourIter = behaviourList.GetEnumerator();
            while (behaviourIter.MoveNext())
            {
                
            }

        }
    }
}
