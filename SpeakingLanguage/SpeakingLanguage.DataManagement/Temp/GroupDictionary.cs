using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.DataManagement
{
    public interface IEntityGroup
    {
        string Subject { get; }
        bool TrySearch(int actorHandle, string valueKey, out int value);
        bool TrySearch(int actorHandle, string valueKey, out float value);
        bool TrySearch(int actorHandle, string valueKey, out string value);
    }

    public class GroupDictionary
    {
        class Group : PoolingObject<Group>, IEntityGroup
        {
            class ValueSet : PoolingObject<ValueSet>
            {
                public Dictionary<string, int> Integer { get; } = new Dictionary<string, int>(new StringRefComparer());
                public Dictionary<string, float> Float { get; } = new Dictionary<string, float>(new StringRefComparer());
                public Dictionary<string, string> String { get; } = new Dictionary<string, string>(new StringRefComparer());

                protected override void OnRelease()
                {
                    Integer.Clear();
                    Float.Clear();
                    String.Clear();
                }
            }

            private Dictionary<int, ValueSet> _setDic = new Dictionary<int, ValueSet>();
            private IEnumerator<KeyValuePair<int, ValueSet>> _setIter = null;

            public string Subject { get; private set; }

            public Group()
            {
                _setIter = _setDic.GetEnumerator();
            }

            public Group Take(string subject)
            {
                Subject = subject;
                return this;
            }
            
            public bool TrySearch(int actorHandle, string valueKey, out int value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = 0;
                    return false;
                }

                return set.Integer.TryGetValue(valueKey, out value);
            }

            public bool TrySearch(int actorHandle, string valueKey, out float value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = default(float);
                    return false;
                }

                return set.Float.TryGetValue(valueKey, out value);
            }

            public bool TrySearch(int actorHandle, string valueKey, out string value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = default(string);
                    return false;
                }

                return set.String.TryGetValue(valueKey, out value);
            }

            public void Insert(int actorHandle)
            {
                _setDic.Add(actorHandle, ValueSet.Create());
            }

            public bool Remove(int actorHandle)
            {
                ValueSet.Destroy(_setDic[actorHandle]);
                return _setDic.Remove(actorHandle);
            }
        }

        private Dictionary<string, Group> _groupDic = new Dictionary<string, Group>(new StringRefComparer());

        public IEntityGroup Search(string subject)
        {
            return _groupDic.ContainsKey(subject) ? _groupDic[subject] : null;
        }

        public void Insert(string subject)
        {
            _groupDic.Add(subject, Group.Create().Take(subject));
        }
    }
}
