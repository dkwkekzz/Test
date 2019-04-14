using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Command
{
    // 다른 방식의 폴링이 필요.
    // 모든것을 미리 할당하고 그에 범위만 제공해주는 형태.
    // allocate는 멀티스레드에 대응해야 한다.
    // -> 할당을 당장하는게 아니라 스레드별 공간에 저장해두었다가, collect에서 라인들을 복사시킨다. 
    // 공간에 들어갈때마다 개별 복사담당 스레드에서 정리한다. 다음 read가 시작되기 전에 아직 완료되지않았다면 read를 멈추어야한다.
    // 근데 이렇게하면 복사하는 비용이 스레드로 돌리는 이점보다 더클듯...?
    class Book : IService
    {
        private const int MAX_LENGTH = 1 << 16;
        private int current;
        public Line[] lines;

        public Book()
        {
            lines = new Line[MAX_LENGTH];
            for (int i = 0; i != MAX_LENGTH; i++)
                lines[i] = new Line();
        }

        public void Initialize(IProvider provider)
        {
            throw new NotImplementedException();
        }

        public int Allocate(int count)
        {
            var temp = current;
            current += count;
            return temp;
        }
    }
}
