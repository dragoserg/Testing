﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Mnogoagentnoe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(49);
            await Task.Run(() => setCells());
            
        }

        private List<DataGridViewCell> Cells = new List<DataGridViewCell>(); //Список со всеми ячейками
        private List<DataGridViewCell> PotentialCustomers = new List<DataGridViewCell>(); //Список со всеми возможными покупателями
        private Dictionary<DataGridViewCell, int> Customers = new Dictionary<DataGridViewCell, int>();
        private List<int> CustomersList = new List<int>();
        private List<int> PotentialCustomersList = new List<int>();
        bool isPopulation = false;
        private Random random = new Random();
        private int day = 0;

        private void setCells() // Метод для записи ячеек в список
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    Cells.Add(dataGridView1.Rows[i].Cells[j]);
                }
            }

        }

        private void clearErrors()
        {
            ErrorLog.Text = string.Empty;
            lblErrorCustomer.Text = string.Empty;
            lblErrorNotice.Text = string.Empty;
            lblErrorPopul.Text = string.Empty;
            lblErrorStoringDays.Text = string.Empty;
            lblErrorDays.Text = string.Empty; 
        }

        private void setPopulation() // Метод для записи ячеек с потенциальными потребителями
        {

            int value = int.Parse(txtPopulation.Text);
            for (int i = 0; i < value; i++)
            {
                var b = Cells[random.Next(Cells.Count)];

                if (!PotentialCustomers.Contains(b)) { PotentialCustomers.Add(b); }
                else { i--; }
            }

            foreach (DataGridViewCell cell in PotentialCustomers)
            {
                cell.Style.BackColor = Color.Yellow;
            }
            isPopulation = true;
        }

        private void newDay()
        {
            int chance = int.Parse(txtChanceBecomeCustomer.Text); //Новые покупатели
            if (int.TryParse(txtChanceNotice.Text, out int notice) && notice >= 0 && notice <= 100)
                            {
                                foreach (var cell in PotentialCustomers) //перебор всех потенциальных покупателей
                                {
                                    if (random.Next(1, 100) <= chance)
                                    {
                                        if (!Customers.Keys.Contains(cell))
                                        {
                                            Customers.Add(cell, -1); // добавление нового покупателя при удачном шансе
                                            if (random.Next(1, 100) <= notice)
                                            {
                                                while(true)
                                                {
                                                    var rnd = PotentialCustomers[random.Next(0, PotentialCustomers.Count)];
                                                    if (!Customers.Keys.Contains(rnd))
                                                    {
                                                        Customers.Add(rnd, -1);
                                                        break;
                                                    }
                                                    
            
                                                }
                                            }
                                        }
                                        
                                    }
                                }
                            }
                            else
                            {
                                lblErrorNotice.Text += "Шанс задан не верно\n";
                            }

            if (int.TryParse(txtStoringDays.Text, out int storingDays) && storingDays > 0) // Удаление покупателей из потенциальных покупателей
            {
                foreach (var cell in Customers.Keys.ToList())
                {
                    cell.Style.BackColor = Color.LightGreen;
                    PotentialCustomers.Remove(cell);
                    Customers[cell] += 1;
                    if (Customers[cell] >= storingDays)
                    {
                        PotentialCustomers.Add(cell);
                        cell.Style.BackColor = Color.Yellow;
                    }
                }
            }
            else
            {
                lblErrorStoringDays.Text += "Неверное значение\n";
                return;
            }


            foreach (var cell in PotentialCustomers)
            {
                if (Customers.Keys.Contains(cell))
                {
                    Customers.Remove(cell);
                }
            }

            PotentialCustomersList.Add(PotentialCustomers.Count());
            CustomersList.Add(Customers.Count());
            day++;
            lblDay.Text = day.ToString();
        }

        private void updateChart()
        {
            // Очистка старых серий, если они существуют
            chart1.Series.Clear();

            // Настройка диапазона осей
            chart1.ChartAreas[0].AxisY.Minimum = 0; // Минимальное значение оси Y

            // Создание серии для потенциальных покупателей
            Series potentialSeries = new Series("Потенциальные покупатели");
            potentialSeries.ChartType = SeriesChartType.Line; // Линейный график
            potentialSeries.Color = Color.Green; // Цвет линии - желтый
            potentialSeries.BorderWidth = 2; // Толщина линии

            // Добавление данных из списка PotentialCustomersList
            for (int i = 0; i < PotentialCustomersList.Count; i++)
            {
                potentialSeries.Points.AddXY(i + 1, PotentialCustomersList[i]);
            }

            // Добавление серии на график
            chart1.Series.Add(potentialSeries);

            // Создание серии для покупателей
            Series customerSeries = new Series("Покупатели");
            customerSeries.ChartType = SeriesChartType.Line; // Линейный график
            customerSeries.Color = Color.Yellow; // Цвет линии - зеленый
            customerSeries.BorderWidth = 2; // Толщина линии

            // Добавление данных из списка CustomersList
            for (int i = 0; i < CustomersList.Count; i++)
            {
                customerSeries.Points.AddXY(i + 1, CustomersList[i]);
            }

            // Добавление серии на график
            chart1.Series.Add(customerSeries);

            // Обновление графика
            chart1.Invalidate();
        }

        private void deletePopulation() // Метод для удаления популяции
        {
            foreach (DataGridViewCell cell in PotentialCustomers)
            {
                cell.Style.BackColor = Color.White;
            }

            foreach (DataGridViewCell cell in Customers.Keys)
            {
                cell.Style.BackColor = Color.White;
            }

            PotentialCustomers.Clear();
            Customers.Clear();
            isPopulation = false;
        }

        private async void start() // Метод начала программы
        {
            if (!isPopulation) 
            {
                await Task.Run(() => setPopulation());
            }

            if (isPopulation)
            {
                if (int.TryParse(txtDaysCount.Text, out int count) && count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        await Task.Run(() => newDay());
                        await Task.Run(() => updateChart());
                    }
                        

                }
                else
                {
                    lblErrorDays.Text = "Неверное количество дней\n";
                    return;
                }
            }
            
        }

        private async void bntStart_Click(object sender, EventArgs e)
        {
            //clearErrors();
            await Task.Run(() => start());
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            //clearErrors();
            await Task.Run(() => deletePopulation());
            foreach (DataGridViewCell cell in Cells)
                cell.Style.BackColor = Color.White;
            PotentialCustomers.Clear();
            Customers.Clear();
            PotentialCustomersList.Clear();
            CustomersList.Clear();
            day = 0;
            lblDay.Text = day.ToString();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            //clearErrors();
            setPopulation();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //clearErrors();
            deletePopulation();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //clearErrors();
        }
    }
}
