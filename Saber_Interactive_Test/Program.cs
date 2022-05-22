using System;
using System.IO;
using System.Collections.Generic;

namespace Saber_Interactive_Test
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            temp = Head;

            //собираем список из имеющихся узлов
            do
            {
                arr.Add(temp);
                temp = temp.Next;
            } while (temp != null);

            //записываем в файл данные узла и индекс на рандомный узел в списке
            using StreamWriter sw = new StreamWriter(s);
            foreach (ListNode ln in arr)
                sw.WriteLine(ln.Data + ":" + arr.IndexOf(ln.Random).ToString());
        }

        public void Deserialize(Stream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            Count = 0;
            Head = temp;
            string line;

            //попытка прочитать файл и создание вспомогательного списка с узлами
            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            Count++;
                            temp.Data = line;
                            ListNode next = new ListNode();
                            temp.Next = next;
                            arr.Add(temp);
                            next.Previous = temp;
                            temp = next;
                        }
                    }
                }

                //назначаем хвост
                Tail = temp.Previous;
                Tail.Next = null;

                //корректируем вспомогательный список, чтобы привести его к изначальному виду
                foreach (ListNode ln in arr)
                {
                    ln.Random = arr[Convert.ToInt32(ln.Data.Split(':')[1])];
                    ln.Data = ln.Data.Split(':')[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось обработать файл данных, возможно, он поврежден, подробности:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.Read();
                Environment.Exit(0);
            }
        }
    }
    internal class Program
    {
        static Random rand = new Random();

        //метод добавления следующего узла в списке
        static ListNode addNode(ListNode prev)
        {
            ListNode result = new ListNode();
            result.Previous = prev;
            result.Next = null;
            result.Data = rand.Next(0, 100).ToString();
            prev.Next = result;
            return result;
        }

        //метод для добавления ссылки на рандомный узел
        static ListNode randomNode(ListNode _head, int _length)
        {
            int k = rand.Next(0, _length);
            int i = 0;
            ListNode result = _head;
            while (i < k)
            {
                result = result.Next;
                i++;
            }
            return result;
        }

        static void Main(string[] args)
        {
            //количество узлов для теста
            int length = 6;

            //первый узел
            ListNode head = new ListNode();
            ListNode tail = new ListNode();
            ListNode temp = new ListNode();

            head.Data = rand.Next(0, 1000).ToString();

            tail = head;

            for (int i = 1; i < length; i++)
                tail = addNode(tail);

            temp = head;

            //добавление ссылки на рандомный узел
            for (int i = 0; i < length; i++)
            {
                temp.Random = randomNode(head, length);
                temp = temp.Next;
            }

            //создание первого списка
            ListRandom first = new ListRandom();
            first.Head = head;
            first.Tail = tail;
            first.Count = length;

            //запись его в файл
            FileStream fs = new FileStream("data.dat", FileMode.OpenOrCreate);
            first.Serialize(fs);

            //чтение первого списка и запись его во второй для проверки
            ListRandom second = new ListRandom();
            try
            {
                fs = new FileStream("data.dat", FileMode.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.Read();
                Environment.Exit(0);
            }
            second.Deserialize(fs);

            //проверка данных последнего узла обоих списков
            if (second.Tail.Data == first.Tail.Data) Console.WriteLine("Success");
            Console.Read();
        }
    }
}
