# PC-Diagnostic-Tool-GUI
PC Diagnostic Tool GUI
# **Документация для Git / Git Documentation**  


### **Проект: PC Diagnostic Tool GUI**  
**Описание**:  
Приложение на Windows Forms для комплексной диагностики ПК, включая информацию об оборудовании, тесты производительности (CPU, GPU, RAM, Disk) и системные отчеты.  


### **Начало работы**  
#### **Требования**  
- .NET 6.0+ SDK ([Скачать](https://dotnet.microsoft.com/download))  
- ОС Windows (тестировалось на Windows 10/11)  
- Git ([Скачать](https://git-scm.com/))  

#### **Клонирование репозитория**  
```bash
git clone https://github.com/your-username/PCDiagnosticToolGUI.git
cd PCDiagnosticToolGUI
```

#### **Сборка проекта**  
```bash
dotnet build
```

#### **Запуск приложения**  
```bash
dotnet run
```

#### **Создание автономного EXE**  
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
*(Результат в `bin/Release/net6.0-windows/win-x64/publish/`)*  

### **Стратегия ветвления**  
- `main` – Стабильные релизы  
- `dev` – Ветка для разработки (в неё вливаются feature-ветки)  
- `feature/*` – Разработка фич (например, `feature/cpu-benchmark`)  

### **Правила коммитов**  
- Используйте **Conventional Commits** (`feat:`, `fix:`, `docs:`, `refactor:`, и т. д.)  
- Пример:  
  ```
  feat: добавлен тест CPU  
  fix: утечка памяти при проверке диска  
  docs: обновлён README с инструкциями сборки  
  ```

### **Участие в разработке**  
1. Форкните репозиторий  
2. Создайте ветку (`git checkout -b feature/ваша-фича`)  
3. Зафиксируйте изменения (`git commit -m "feat: ваша фича"`)  
4. Запушьте ветку (`git push origin feature/ваша-фича`)  
5. Откройте **Pull Request**  

---


📌 **Примечание**:  
- Для работы некоторых функций (например, диагностики диска) могут потребоваться **права администратора**.  
- Приложение **работает только на Windows**.  
