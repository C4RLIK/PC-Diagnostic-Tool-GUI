using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PCDiagnosticToolGUI
{
    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll")]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        private RichTextBox txtOutput;
        private ProgressBar progressBar;
        private Button btnScan;
        private Button btnSaveReport;
        private Button btnCpuBenchmark;
        private Button btnGpuBenchmark;
        private Button btnRamBenchmark;
        private Button btnDiskBenchmark;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Комплексная диагностика ПК";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Кнопка сканирования
            btnScan = new Button
            {
                Text = "Сканировать систему",
                Location = new Point(20, 20),
                Size = new Size(200, 40),
                Font = new Font("Arial", 10)
            };
            btnScan.Click += BtnScan_Click;

            // Кнопка сохранения отчета
            btnSaveReport = new Button
            {
                Text = "Сохранить отчет",
                Location = new Point(20, 70),
                Size = new Size(200, 40),
                Font = new Font("Arial", 10),
                Enabled = false
            };
            btnSaveReport.Click += BtnSaveReport_Click;

            // Поле вывода
            txtOutput = new RichTextBox
            {
                Location = new Point(240, 20),
                Size = new Size(530, 520),
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Прогресс-бар
            progressBar = new ProgressBar
            {
                Location = new Point(20, 120),
                Size = new Size(200, 30),
                Style = ProgressBarStyle.Continuous
            };

            // Кнопки бенчмарков
            btnCpuBenchmark = new Button
            {
                Text = "Тест CPU",
                Location = new Point(20, 170),
                Size = new Size(200, 30),
                Font = new Font("Arial", 9)
            };
            btnCpuBenchmark.Click += BtnCpuBenchmark_Click;

            btnGpuBenchmark = new Button
            {
                Text = "Тест GPU",
                Location = new Point(20, 210),
                Size = new Size(200, 30),
                Font = new Font("Arial", 9)
            };
            btnGpuBenchmark.Click += BtnGpuBenchmark_Click;

            btnRamBenchmark = new Button
            {
                Text = "Тест RAM",
                Location = new Point(20, 250),
                Size = new Size(200, 30),
                Font = new Font("Arial", 9)
            };
            btnRamBenchmark.Click += BtnRamBenchmark_Click;

            btnDiskBenchmark = new Button
            {
                Text = "Тест диска",
                Location = new Point(20, 290),
                Size = new Size(200, 30),
                Font = new Font("Arial", 9)
            };
            btnDiskBenchmark.Click += BtnDiskBenchmark_Click;

            this.Controls.Add(btnScan);
            this.Controls.Add(btnSaveReport);
            this.Controls.Add(txtOutput);
            this.Controls.Add(progressBar);
            this.Controls.Add(btnCpuBenchmark);
            this.Controls.Add(btnGpuBenchmark);
            this.Controls.Add(btnRamBenchmark);
            this.Controls.Add(btnDiskBenchmark);
        }

        private void BtnScan_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
            progressBar.Value = 0;
            btnSaveReport.Enabled = false;

            txtOutput.AppendText("=== НАЧАЛО ДИАГНОСТИКИ ===\n\n");
            progressBar.Value = 5;

            try
            {
                // Системная информация
                GetSystemInfo();
                progressBar.Value = 15;

                // Аппаратная информация
                GetHardwareInfo();
                progressBar.Value = 50;

                // Дисковая подсистема
                GetStorageInfo();
                progressBar.Value = 70;

                // Сетевая информация
                GetNetworkInfo();
                progressBar.Value = 85;

                // Производительность
                GetPerformanceInfo();
                progressBar.Value = 95;

                txtOutput.AppendText("\n=== ДИАГНОСТИКА ЗАВЕРШЕНА ===\n");
                btnSaveReport.Enabled = true;
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОШИБКА: {ex.Message}\n");
            }

            progressBar.Value = 100;
        }

        private void BtnSaveReport_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
                Title = "Сохранить отчет диагностики",
                FileName = $"PC_Diagnostic_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, txtOutput.Text);
                    MessageBox.Show("Отчет успешно сохранен!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GetSystemInfo()
        {
            txtOutput.AppendText("=== СИСТЕМНАЯ ИНФОРМАЦИЯ ===\n");
            
            // Информация об ОС
            txtOutput.AppendText($"ОС: {Environment.OSVersion.VersionString}\n");
            txtOutput.AppendText($"Версия .NET: {Environment.Version}\n");
            txtOutput.AppendText($"Имя компьютера: {Environment.MachineName}\n");
            txtOutput.AppendText($"Имя пользователя: {Environment.UserName}\n");
            txtOutput.AppendText($"Системная папка: {Environment.SystemDirectory}\n");
            txtOutput.AppendText($"Количество процессоров: {Environment.ProcessorCount}\n");
            txtOutput.AppendText($"Система: {(Environment.Is64BitOperatingSystem ? "64-битная" : "32-битная")}\n");
            
            // Время работы системы
            try
            {
                using PerformanceCounter uptimeCounter = new PerformanceCounter("System", "System Up Time");
                uptimeCounter.NextValue();
                TimeSpan uptime = TimeSpan.FromSeconds(uptimeCounter.NextValue());
                txtOutput.AppendText($"Время работы системы: {uptime.Days} дн. {uptime.Hours} ч. {uptime.Minutes} мин.\n\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Ошибка получения времени работы системы: {ex.Message}\n\n");
            }
        }

        private void GetHardwareInfo()
        {
            txtOutput.AppendText("=== АППАРАТНОЕ ОБЕСПЕЧЕНИЕ ===\n");

            // Процессор
            string cpu = GetCpuInfo() ?? "Не удалось определить";
            txtOutput.AppendText($"\nПроцессор:\n{cpu}\n");

            // Видеокарта
            string gpu = GetGpuInfo() ?? "Не удалось определить";
            txtOutput.AppendText($"\nВидеокарта:\n{gpu}\n");

            // Память
            var (totalGB, freeGB, memoryLoad) = GetMemoryInfo();
            txtOutput.AppendText($"\nОперативная память:\nВсего: {totalGB} GB\nСвободно: {freeGB} GB\nИспользовано: {memoryLoad}%\n");

            // Материнская плата
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (ManagementObject obj in searcher.Get())
                {
                    txtOutput.AppendText($"\nМатеринская плата:\nПроизводитель: {obj["Manufacturer"]}\nМодель: {obj["Product"]}\n");
                }
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОшибка получения информации о материнской плате: {ex.Message}\n");
            }

            // BIOS
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (ManagementObject obj in searcher.Get())
                {
                    txtOutput.AppendText($"\nBIOS:\nПроизводитель: {obj["Manufacturer"]}\nВерсия: {obj["Version"]}\nДата: {obj["ReleaseDate"]}\n");
                }
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОшибка получения информации о BIOS: {ex.Message}\n");
            }
        }

        private void GetStorageInfo()
        {
            txtOutput.AppendText("\n=== ДИСКОВАЯ ПОДСИСТЕМА ===\n");

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives.Where(d => d.IsReady))
                {
                    txtOutput.AppendText($"\nДиск {drive.Name}:\n");
                    txtOutput.AppendText($"Тип: {drive.DriveType}\n");
                    txtOutput.AppendText($"Файловая система: {drive.DriveFormat}\n");
                    txtOutput.AppendText($"Метка тома: {drive.VolumeLabel}\n");
                    txtOutput.AppendText($"Общий размер: {drive.TotalSize / (1024 * 1024 * 1024)} GB\n");
                    txtOutput.AppendText($"Свободно: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} GB\n");

                    if (drive.DriveType == DriveType.Fixed)
                    {
                        var diskInfo = GetDiskPerformance(drive.Name[0].ToString());
                        txtOutput.AppendText($"Тип накопителя: {diskInfo.Type}\n");
                        txtOutput.AppendText($"Скорость: {diskInfo.Speed} MB/s\n");
                    }
                }
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОшибка получения информации о дисках: {ex.Message}\n");
            }
        }

        private void GetNetworkInfo()
        {
            txtOutput.AppendText("\n=== СЕТЕВАЯ ИНФОРМАЦИЯ ===\n");

            // Интернет соединение
            bool isConnected = InternetGetConnectedState(out int desc, 0);
            txtOutput.AppendText($"Интернет: {(isConnected ? "Подключен" : "Отключен")}\n");

            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    txtOutput.AppendText($"\nСетевой адаптер: {obj["Description"]}\n");
                    txtOutput.AppendText($"MAC адрес: {obj["MACAddress"]}\n");

                    string[] ips = (string[])obj["IPAddress"];
                    string[] subnets = (string[])obj["IPSubnet"];
                    if (ips != null && ips.Length > 0)
                    {
                        txtOutput.AppendText($"IP адрес: {string.Join(", ", ips)}\n");
                        txtOutput.AppendText($"Маска подсети: {string.Join(", ", subnets)}\n");
                    }

                    string[] gateways = (string[])obj["DefaultIPGateway"];
                    if (gateways != null && gateways.Length > 0)
                    {
                        txtOutput.AppendText($"Шлюз: {string.Join(", ", gateways)}\n");
                    }

                    string[] dns = (string[])obj["DNSServerSearchOrder"];
                    if (dns != null && dns.Length > 0)
                    {
                        txtOutput.AppendText($"DNS: {string.Join(", ", dns)}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОшибка получения сетевой информации: {ex.Message}\n");
            }
        }

        private void GetPerformanceInfo()
        {
            txtOutput.AppendText("\n=== ПРОИЗВОДИТЕЛЬНОСТЬ ===\n");

            try
            {
                // Загрузка CPU
                using PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(1000);
                float cpuUsage = cpuCounter.NextValue();
                txtOutput.AppendText($"Загрузка CPU: {cpuUsage:F1}%\n");

                // Доступная память
                MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX() { dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)) };
                if (GlobalMemoryStatusEx(ref memStatus))
                {
                    txtOutput.AppendText($"Использование памяти: {memStatus.dwMemoryLoad}%\n");
                }

                // Дисковые операции
                using PerformanceCounter diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                using PerformanceCounter diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
                float diskRead = diskReadCounter.NextValue() / 1024f;
                float diskWrite = diskWriteCounter.NextValue() / 1024f;
                txtOutput.AppendText($"Дисковая активность: Чтение {diskRead:F1} KB/s | Запись {diskWrite:F1} KB/s\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"\nОшибка получения информации о производительности: {ex.Message}\n");
            }
        }

        private static string? GetCpuInfo()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                return key?.GetValue("ProcessorNameString")?.ToString()?.Trim();
            }
            catch
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["Name"]?.ToString();
                    }
                }
                catch
                {
                    return null;
                }
                return null;
            }
        }

        private static (ulong TotalGB, ulong FreeGB, uint MemoryLoad) GetMemoryInfo()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX))
            };

            if (GlobalMemoryStatusEx(ref memStatus))
            {
                return (memStatus.ullTotalPhys / (1024 * 1024 * 1024), 
                       memStatus.ullAvailPhys / (1024 * 1024 * 1024),
                       memStatus.dwMemoryLoad);
            }
            
            // Альтернативный способ через WMI
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong total = Convert.ToUInt64(obj["TotalPhysicalMemory"]) / (1024 * 1024 * 1024);
                    return (total, 0, 0);
                }
            }
            catch
            {
                return (0, 0, 0);
            }
            return (0, 0, 0);
        }

        private static (string Type, int Speed) GetDiskPerformance(string driveLetter)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), $"disk_test_{Guid.NewGuid()}.tmp");
                byte[] data = new byte[100 * 1024 * 1024]; // 100 MB тестовых данных
                new Random().NextBytes(data);
                
                // Тест записи
                var sw = Stopwatch.StartNew();
                File.WriteAllBytes(tempFile, data);
                sw.Stop();
                double writeSpeed = data.Length / (sw.Elapsed.TotalSeconds * 1024 * 1024);
                
                // Тест чтения
                sw.Restart();
                byte[] readData = File.ReadAllBytes(tempFile);
                sw.Stop();
                double readSpeed = data.Length / (sw.Elapsed.TotalSeconds * 1024 * 1024);
                
                File.Delete(tempFile);
                
                int avgSpeed = (int)((writeSpeed + readSpeed) / 2);
                bool isSSD = avgSpeed > 200;
                
                return (isSSD ? "SSD (вероятно)" : "HDD (вероятно)", avgSpeed);
            }
            catch
            {
                try
                {
                    // Альтернативный способ определения типа диска
                    using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_DiskDrive WHERE DeviceID LIKE '%{driveLetter}%'");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string mediaType = obj["MediaType"]?.ToString() ?? "";
                        if (mediaType.Contains("Fixed") || mediaType.Contains("SSD"))
                            return ("SSD", 0);
                        return ("HDD", 0);
                    }
                }
                catch
                {
                    return ("Неизвестно", 0);
                }
                return ("Неизвестно", 0);
            }
        }

        private static string? GetGpuInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                var gpuInfo = string.Join("\n", searcher.Get()
                    .Cast<ManagementObject>()
                    .Select(gpu => $"{gpu["Name"]} (VRAM: {Convert.ToInt64(gpu["AdapterRAM"]) / (1024 * 1024)} MB)"));

                return string.IsNullOrEmpty(gpuInfo) ? null : gpuInfo;
            }
            catch
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winsat");
                    return key?.GetValue("PrimaryAdapterString")?.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }

        private async void BtnCpuBenchmark_Click(object sender, EventArgs e)
        {
            btnCpuBenchmark.Enabled = false;
            txtOutput.AppendText("\n=== ТЕСТ ПРОИЗВОДИТЕЛЬНОСТИ CPU ===\n");

            try
            {
                // Тест скорости вычислений с плавающей точкой
                txtOutput.AppendText("Тест FPU (флоаты)... ");
                var fpuTime = await Task.Run(() => MeasureFloatingPointTest());
                txtOutput.AppendText($"{fpuTime:F2} ms\n");

                // Тест скорости целочисленных вычислений
                txtOutput.AppendText("Тест целочисленных операций... ");
                var intTime = await Task.Run(() => MeasureIntegerTest());
                txtOutput.AppendText($"{intTime:F2} ms\n");

                // Тест скорости вычислений с двойной точностью
                txtOutput.AppendText("Тест DPU (даблы)... ");
                var dpuTime = await Task.Run(() => MeasureDoubleTest());
                txtOutput.AppendText($"{dpuTime:F2} ms\n");

                // Тест скорости хэширования
                txtOutput.AppendText("Тест хэширования... ");
                var hashTime = await Task.Run(() => MeasureHashTest());
                txtOutput.AppendText($"{hashTime:F2} ms\n");

                // Общая оценка
                double score = 100000 / (fpuTime + intTime + dpuTime + hashTime);
                txtOutput.AppendText($"\nОбщая оценка CPU: {score:F2} баллов\n");

                txtOutput.AppendText("Тест CPU завершен успешно!\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Ошибка: {ex.Message}\n");
            }
            finally
            {
                btnCpuBenchmark.Enabled = true;
            }
        }

        private long MeasureFloatingPointTest()
        {
            float a = 0.5f;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000000; i++)
            {
                a = a * 1.000001f + 0.000001f;
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long MeasureIntegerTest()
        {
            int a = 1;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000000; i++)
            {
                a = a + a * 2 - 1;
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long MeasureDoubleTest()
        {
            double a = 0.5;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000000; i++)
            {
                a = a * 1.000001 + 0.000001;
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long MeasureHashTest()
        {
            string test = "Benchmark test string";
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                test.GetHashCode();
                test += "x";
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private async void BtnGpuBenchmark_Click(object sender, EventArgs e)
        {
            btnGpuBenchmark.Enabled = false;
            txtOutput.AppendText("\n=== ТЕСТ ПРОИЗВОДИТЕЛЬНОСТИ GPU ===\n");

            try
            {
                txtOutput.AppendText("Тест рендеринга... ");
                var renderTime = await Task.Run(() => MeasureGpuRenderingTest());
                txtOutput.AppendText($"{renderTime:F2} ms\n");

                txtOutput.AppendText("Тест вычислений... ");
                var calcTime = await Task.Run(() => MeasureGpuCalculationTest());
                txtOutput.AppendText($"{calcTime:F2} ms\n");

                // Общая оценка
                double score = 10000 / (renderTime + calcTime);
                txtOutput.AppendText($"\nОбщая оценка GPU: {score:F2} баллов\n");

                txtOutput.AppendText("Тест GPU завершен успешно!\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Ошибка: {ex.Message}\n");
            }
            finally
            {
                btnGpuBenchmark.Enabled = true;
            }
        }

        private long MeasureGpuRenderingTest()
        {
            var sw = Stopwatch.StartNew();
            using (var bmp = new Bitmap(1000, 1000))
            using (var g = Graphics.FromImage(bmp))
            {
                var rnd = new Random();
                for (int i = 0; i < 1000; i++)
                {
                    g.DrawEllipse(Pens.Red, 
                        rnd.Next(1000), rnd.Next(1000), 
                        rnd.Next(100, 200), rnd.Next(100, 200));
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long MeasureGpuCalculationTest()
        {
            Vector4 v1 = new Vector4(0.5f, 0.7f, 1.0f, 0.3f);
            Vector4 v2 = new Vector4(1.0f, 0.1f, 0.3f, 0.8f);
            var sw = Stopwatch.StartNew();
            
            for (int i = 0; i < 10000000; i++)
            {
                v1 = Vector4.Lerp(v1, v2, 0.01f);
                var len = v1.Length();
            }
            
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private async void BtnRamBenchmark_Click(object sender, EventArgs e)
        {
            btnRamBenchmark.Enabled = false;
            txtOutput.AppendText("\n=== ТЕСТ ПРОИЗВОДИТЕЛЬНОСТИ RAM ===\n");

            try
            {
                txtOutput.AppendText("Тест записи... ");
                var writeSpeed = await Task.Run(() => MeasureRamWriteSpeed());
                txtOutput.AppendText($"{writeSpeed:F2} MB/s\n");

                txtOutput.AppendText("Тест чтения... ");
                var readSpeed = await Task.Run(() => MeasureRamReadSpeed());
                txtOutput.AppendText($"{readSpeed:F2} MB/s\n");

                txtOutput.AppendText("Тест задержки... ");
                var latency = await Task.Run(() => MeasureRamLatency());
                txtOutput.AppendText($"{latency:F2} ns\n");

                // Общая оценка
                double score = (writeSpeed + readSpeed) / 100 + 1000 / latency;
                txtOutput.AppendText($"\nОбщая оценка RAM: {score:F2} баллов\n");

                txtOutput.AppendText("Тест RAM завершен успешно!\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Ошибка: {ex.Message}\n");
            }
            finally
            {
                btnRamBenchmark.Enabled = true;
            }
        }

        private double MeasureRamWriteSpeed()
        {
            const int size = 100 * 1024 * 1024; // 100 MB
            byte[] data = new byte[size];
            new Random().NextBytes(data);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i += 4096) // Запись блоками по 4KB
            {
                data[i] = (byte)(i % 256);
            }
            sw.Stop();

            return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
        }

        private double MeasureRamReadSpeed()
        {
            const int size = 100 * 1024 * 1024; // 100 MB
            byte[] data = new byte[size];
            new Random().NextBytes(data);

            long sum = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i += 4096) // Чтение блоками по 4KB
            {
                sum += data[i];
            }
            sw.Stop();

            return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
        }

        private double MeasureRamLatency()
        {
            const int size = 10 * 1024 * 1024; // 10 MB
            byte[] data = new byte[size];
            new Random().NextBytes(data);

            long sum = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                sum += data[(i * 64) % size]; // Доступ с шагом 64 байта (типичный размер кэш-линии)
            }
            sw.Stop();

            return (sw.Elapsed.TotalNanoseconds / 1000000);
        }

        private async void BtnDiskBenchmark_Click(object sender, EventArgs e)
        {
            btnDiskBenchmark.Enabled = false;
            txtOutput.AppendText("\n=== ТЕСТ ПРОИЗВОДИТЕЛЬНОСТИ ДИСКА ===\n");

            try
            {
                txtOutput.AppendText("Тест последовательной записи... ");
                var seqWrite = await Task.Run(() => MeasureDiskSequentialWriteSpeed());
                txtOutput.AppendText($"{seqWrite:F2} MB/s\n");

                txtOutput.AppendText("Тест последовательного чтения... ");
                var seqRead = await Task.Run(() => MeasureDiskSequentialReadSpeed());
                txtOutput.AppendText($"{seqRead:F2} MB/s\n");

                txtOutput.AppendText("Тест случайной записи... ");
                var randWrite = await Task.Run(() => MeasureDiskRandomWriteSpeed());
                txtOutput.AppendText($"{randWrite:F2} MB/s\n");

                txtOutput.AppendText("Тест случайного чтения... ");
                var randRead = await Task.Run(() => MeasureDiskRandomReadSpeed());
                txtOutput.AppendText($"{randRead:F2} MB/s\n");

                // Общая оценка
                double score = (seqWrite + seqRead + randWrite + randRead) / 4;
                txtOutput.AppendText($"\nСредняя скорость диска: {score:F2} MB/s\n");

                txtOutput.AppendText("Тест диска завершен успешно!\n");
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Ошибка: {ex.Message}\n");
            }
            finally
            {
                btnDiskBenchmark.Enabled = true;
            }
        }

        private double MeasureDiskSequentialWriteSpeed()
        {
            string tempFile = Path.GetTempFileName();
            const int size = 100 * 1024 * 1024; // 100 MB
            byte[] data = new byte[size];
            new Random().NextBytes(data);

            var sw = Stopwatch.StartNew();
            File.WriteAllBytes(tempFile, data);
            sw.Stop();

            File.Delete(tempFile);
            return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
        }

        private double MeasureDiskSequentialReadSpeed()
        {
            string tempFile = Path.GetTempFileName();
            const int size = 100 * 1024 * 1024; // 100 MB
            byte[] data = new byte[size];
            new Random().NextBytes(data);
            File.WriteAllBytes(tempFile, data);

            var sw = Stopwatch.StartNew();
            byte[] readData = File.ReadAllBytes(tempFile);
            sw.Stop();

            File.Delete(tempFile);
            return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
        }

        private double MeasureDiskRandomWriteSpeed()
        {
            string tempFile = Path.GetTempFileName();
            const int size = 100 * 1024 * 1024; // 100 MB
            const int blockSize = 4096; // 4 KB
            byte[] data = new byte[blockSize];
            new Random().NextBytes(data);

            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, blockSize))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < size / blockSize; i++)
                {
                    fs.Seek(i * blockSize, SeekOrigin.Begin);
                    fs.Write(data, 0, blockSize);
                }
                sw.Stop();
                return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
            }
        }

        private double MeasureDiskRandomReadSpeed()
        {
            string tempFile = Path.GetTempFileName();
            const int size = 100 * 1024 * 1024; // 100 MB
            const int blockSize = 4096; // 4 KB
            byte[] data = new byte[size];
            new Random().NextBytes(data);
            File.WriteAllBytes(tempFile, data);

            using (var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.None, blockSize))
            {
                byte[] buffer = new byte[blockSize];
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < size / blockSize; i++)
                {
                    fs.Seek(i * blockSize, SeekOrigin.Begin);
                    fs.Read(buffer, 0, blockSize);
                }
                sw.Stop();
                return size / (sw.Elapsed.TotalSeconds * 1024 * 1024);
            }
        }
    }
}