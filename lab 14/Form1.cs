using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace lab_14
{
    public partial class Form1 : Form
    {
        private Bank bank;  // Bank object to manage clients and operators
        private int clientId;  // Counter for client IDs
        private Random random;  // Random object for generating random numbers

        public Form1()
        {
            InitializeComponent();
            InitializeSimulation();  // Initialize the simulation setup
        }

        // Method to initialize the simulation parameters
        private void InitializeSimulation()
        {
            bank = new Bank(3);  // Initialize the bank with 3 operators
            clientId = 1;  // Start client IDs from 1
            random = new Random();  // Create a new Random object
            InitializeDataGridView();  // Setup the DataGridView
        }

        // Method to set up the DataGridView columns
        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 3;  // Set the number of columns
            dataGridView1.Columns[0].Name = "Client ID";  // First column for Client IDs
            dataGridView1.Columns[1].Name = "Operator ID";  // Second column for Operator IDs
            dataGridView1.Columns[2].Name = "Status";  // Third column for status (waiting/serving)
        }

        // Event handler for the Start button click
        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Start();  // Start the timer for the simulation
        }

        // Event handler for each timer tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Randomly add new clients to the queue
            if (random.NextDouble() < 0.7)  // 70% chance a new client arrives each second
            {
                bank.AddClient(new Client(clientId++, DateTime.Now));  // Add a new client with a unique ID and current arrival time
            }

            // Process the queue and update operator statuses
            bank.ProcessQueue();
            bank.CheckOperators();

            // Update the DataGridView to reflect the current state
            UpdateDataGridView();
        }

        // Method to update the DataGridView with current client and operator statuses
        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();  // Clear existing rows

            // Add rows for clients in the queue
            foreach (var client in bank.GetQueue())
            {
                dataGridView1.Rows.Add(client.Id, "", "Waiting");  // Client is waiting, no operator assigned
            }

            // Add rows for operators and their statuses
            foreach (var op in bank.GetOperators())
            {
                if (op.IsBusy)
                {
                    // Operator is busy, show the client being served and when the operator will be free
                    dataGridView1.Rows.Add("", op.Id, $"Serving until {op.FreeAt:HH:mm:ss}");
                }
                else
                {
                    // Operator is free
                    dataGridView1.Rows.Add("", op.Id, "Free");
                }
            }
        }
    }

    // Client class to represent a bank client
    public class Client
    {
        public int Id { get; set; }
        public DateTime ArrivalTime { get; set; }

        public Client(int id, DateTime arrivalTime)
        {
            Id = id;  // Assign client ID
            ArrivalTime = arrivalTime;  // Assign arrival time
        }
    }

    // Operator class to represent a bank operator
    public class Operator
    {
        public int Id { get; set; }
        public bool IsBusy { get; set; }
        public DateTime FreeAt { get; set; }

        public Operator(int id)
        {
            Id = id;  // Assign operator ID
            IsBusy = false;  // Initially, the operator is not busy
        }

        // Method to start serving a client
        public void ServeClient(Client client, int serviceTime)
        {
            IsBusy = true;  // Mark the operator as busy
            FreeAt = DateTime.Now.AddSeconds(serviceTime);  // Set the time when the operator will be free
        }

        // Method to mark the operator as free
        public void Free()
        {
            IsBusy = false;  // Mark the operator as free
        }
    }

    // Bank class to manage the clients and operators
    public class Bank
    {
        private Queue<Client> queue = new Queue<Client>();  // Queue to manage waiting clients
        private List<Operator> operators = new List<Operator>();  // List of operators

        public Bank(int operatorCount)
        {
            for (int i = 0; i < operatorCount; i++)
            {
                operators.Add(new Operator(i + 1));  // Initialize the specified number of operators
            }
        }

        // Method to add a client to the queue
        public void AddClient(Client client)
        {
            queue.Enqueue(client);  // Add client to the queue
            Console.WriteLine($"Client {client.Id} arrived at {client.ArrivalTime}");
        }

        // Method to process the queue and assign clients to free operators
        public void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                var client = queue.Peek();  // Get the client at the front of the queue
                var freeOperator = operators.FirstOrDefault(op => !op.IsBusy);  // Find a free operator

                if (freeOperator != null)
                {
                    int serviceTime = new Random().Next(5, 15);  // Simulating service time between 5 to 15 seconds
                    freeOperator.ServeClient(client, serviceTime);  // Assign client to the operator
                    queue.Dequeue();  // Remove client from the queue
                    Console.WriteLine($"Client {client.Id} is being served by Operator {freeOperator.Id} for {serviceTime} seconds.");
                }
                else
                {
                    break;  // No free operators, exit the loop
                }
            }
        }

        // Method to check the status of each operator and free them if their service time is over
        public void CheckOperators()
        {
            foreach (var op in operators)
            {
                if (op.IsBusy && DateTime.Now >= op.FreeAt)
                {
                    op.Free();  // Free the operator if the service time is over
                    Console.WriteLine($"Operator {op.Id} is now free.");
                }
            }
        }

        // Method to get the current queue of clients
        public List<Client> GetQueue()
        {
            return queue.ToList();  // Return the queue as a list
        }

        // Method to get the list of operators
        public List<Operator> GetOperators()
        {
            return operators;  // Return the list of operators
        }
    }
}
