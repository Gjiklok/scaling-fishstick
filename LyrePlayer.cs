using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using WindowsInput;
using WindowsInput.Native;

namespace LyrePlayer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ШАГ 1: Приветствие
            Console.WriteLine("═══════════════════════════════════════════════\n");
            Console.WriteLine("  Проигрыватель для лиры Genshin Impact");
            Console.WriteLine("═══════════════════════════════════════════════\n");

            // ШАГ 2: Запрос пути к MIDI файлу
            Console.Write("Введите путь к MIDI файлу: ");
            string? midiPath = Console.ReadLine();

            // TODO: Проверить, что путь не пустой и файл существует
            // Подсказка: используй File.Exists(midiPath)
            if (string.IsNullOrWhiteSpace(midiPath) || !File.Exists(midiPath))
            {
                Console.WriteLine("Ошибка: Файл не найден!\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            // ШАГ 3: Загрузка MIDI файла
            MidiFile midiFile;
            try
            {
                // TODO: Загрузить MIDI файл
                // Подсказка: midiFile = MidiFile.Read(midiPath);
                midiFile = MidiFile.Read(midiPath);
                Console.WriteLine($"✓ Файл успешно загружен: {Path.GetFileName(midiPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            // ШАГ 4: Подготовка к воспроизведению
            Console.WriteLine("\nИнструкция:");
            Console.WriteLine("1. Запустите Genshin Impact");
            Console.WriteLine("2. Войдите в игру и возьмите лиру (нажмите Z)");
            Console.WriteLine("3. Откройте интерфейс лиры");
            Console.WriteLine("\nНажмите Enter, когда будете готовы...");
            Console.ReadLine();

            Console.WriteLine("\nНачинаем воспроизведение через 3 секунды...");
            Thread.Sleep(3000);

            // ШАГ 5: Воспроизведение
            PlayMidiOnLyre(midiFile);

            Console.WriteLine("\n═══════════════════════════════════════════════\n Воспроизведение завершено \n═══════════════════════════════════════════════\n");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод для воспроизведения MIDI файла на лире
        /// </summary>
        static void PlayMidiOnLyre(MidiFile midiFile)
        {
            // Создаём симулятор клавиатуры
            var simulator = new InputSimulator();

            // TODO: Получить все ноты из MIDI файла
            // Подсказка: используй GetNotes() и GetTimedEventsAndNotes()
            var tempoMap = midiFile.GetTempoMap();
            var notes = midiFile.GetNotes();

            Console.WriteLine($"Всего нот в файле: {notes.Count()}\nНачинаем играть...");
            Console.WriteLine();

            // Переменная для отслеживания времени в миллисекундах
            double previousTime = 0;

            // Счетчики для статистики
            int totalNotes = notes.Count();
            int playedNotes = 0;
            int skippedNotes = 0;

            // Проходим по всем нотам и играем их
            foreach (var note in notes)
            {
                // Получаем номер MIDI ноты (от 0 до 127)
                int midiNoteNumber = note.NoteNumber;

                // Проверяем, что нота входит в диапазон лиры (48-68)
                if (midiNoteNumber < 48 || midiNoteNumber > 68)
                {
                    skippedNotes++;
                    continue; // Пропускаем ноты вне диапазона лиры
                }

                // Рассчитываем задержку между нотами в миллисекундах
                var currentTime = note.TimeAs<MetricTimeSpan>(tempoMap).TotalMilliseconds;
                var delay = currentTime - previousTime;
                previousTime = currentTime;

                // Ждём перед нажатием клавиши
                if (delay > 0)
                {
                    Thread.Sleep((int)delay);
                }

                // Преобразуем MIDI ноту в клавишу
                VirtualKeyCode? key = NoteMapper.GetKeyForNote(midiNoteNumber);

                if (key.HasValue)
                {
                    // Нажимаем клавишу
                    simulator.Keyboard.KeyPress(key.Value);
                    playedNotes++;
                    Console.WriteLine($"♪ Нота {midiNoteNumber} ({NoteMapper.GetNoteName(midiNoteNumber)}) → клавиша {key.Value}");
                }
            }

            // Выводим статистику
            Console.WriteLine();
            Console.WriteLine($"\nСтатистика:\n Всего нот: {totalNotes}\n  Сыграно: {playedNotes}\n  Пропущено (вне диапазона): {skippedNotes}");
        }
    }
}

