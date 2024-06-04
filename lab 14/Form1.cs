using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace lab_14
{
    public partial class Form1 : Form
    {
        private Bank bank;
        private int clientId;
        private Random random;

        public Form1()
        {
            InitializeComponent();
            InitializeSimulation();
        }

        private void InitializeSimulation()
        {
            bank = new Bank(3); // 3 operators
            clientId = 1;
            random = new Random();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Client ID";
            dataGridView1.Columns[1].Name = "Operator ID";
            dataGridView1.Columns[2].Name = "Status";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (random.NextDouble() < 0.7) // 70% chance a new client arrives each second
            {
                bank.AddClient(new Client(clientId++, DateTime.Now));
            }
            bank.ProcessQueue();
            bank.CheckOperators();
            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();

            foreach (var client in bank.GetQueue())
            {
                dataGridView1.Rows.Add(client.Id, "", "Waiting");
            }

            foreach (var op in bank.GetOperators())
            {
                if (op.IsBusy)
                {
                    dataGridView1.Rows.Add("", op.Id, "Serving");
                }
                else
                {
                    dataGridView1.Rows.Add("", op.Id, "Free");
                }
            }
        }
    }

    public class Client
    {
        public int Id { get; set; }
        public DateTime ArrivalTime { get; set; }

        public Client(int id, DateTime arrivalTime)
        {
            Id = id;
            ArrivalTime = arrivalTime;
        }
    }

    public class Operator
    {
        public int Id { get; set; }
        public bool IsBusy { get; set; }
        public DateTime FreeAt { get; set; }

        public Operator(int id)
        {
            Id = id;
            IsBusy = false;
        }

        public void ServeClient(Client client, int serviceTime)
        {
            IsBusy = true;
            FreeAt = DateTime.Now.AddSeconds(serviceTime);
        }

        public void Free()
        {
            IsBusy = false;
        }
    }

    public class Bank
    {
        private Queue<Client> queue = new Queue<Client>();
        private List<Operator> operators = new List<Operator>();

        public Bank(int operatorCount)
        {
            for (int i = 0; i < operatorCount; i++)
            {
                operators.Add(new Operator(i + 1));
            }
        }

        public void AddClient(Client client)
        {
            queue.Enqueue(client);
            Console.WriteLine($"Client {client.Id} arrived at {client.ArrivalTime}");
        }

        public void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                var client = queue.Peek();
                var freeOperator = operators.FirstOrDefault(op => !op.IsBusy);

                if (freeOperator != null)
                {
                    int serviceTime = new Random().Next(5, 15); // Simulating service time between 5 to 15 seconds
                    freeOperator.ServeClient(client, serviceTime);
                    queue.Dequeue();
                    Console.WriteLine($"Client {client.Id} is being served by Operator {freeOperator.Id} for {serviceTime} seconds.");
                }
                else
                {
                    break;
                }
            }
        }

        public void CheckOperators()
        {
            foreach (var op in operators)
            {
                if (op.IsBusy && DateTime.Now >= op.FreeAt)
                {
                    op.Free();
                    Console.WriteLine($"Operator {op.Id} is now free.");
                }
            }
        }

        public List<Client> GetQueue()
        {
            return queue.ToList();
        }

        public List<Operator> GetOperators()
        {
            return operators;
        }
    }
}
