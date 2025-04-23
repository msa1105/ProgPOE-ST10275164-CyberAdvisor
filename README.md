```markdown
# 🧠 CyberAdvisor - Console-Based Cybersecurity Chatbot

CyberAdvisor is a console-based C# chatbot application designed to simulate a basic cybersecurity assistant. It interacts with users, greets them with a voice prompt and ASCII logo, collects the user’s name for personalization, and answers cybersecurity-related queries with predefined intelligent responses.

---

## 📌 Features

- ASCII logo display using [Figgle](https://www.nuget.org/packages/Figgle/)
- Welcome audio using `System.Media.SoundPlayer`
- Personalized responses using user-provided name
- Basic chatbot functionality with over 20 question-response pairs
- Handles keywords in questions using flexible `Contains()` checks
- Exit functionality using `"exit"` keyword
- GitHub Actions CI workflow for build and syntax checks

---

## 🛠 Technologies Used

- **C# (.NET 6 or later)**
- **Visual Studio 2022**
- **Figgle** (ASCII font rendering)
- **System.Media** (audio playback)
- **GitHub Actions** (CI setup)

---

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2022
- .NET SDK 6 or newer
- Git (for version control)

### Setup

1. Clone the repository:

```bash
git clone https://github.com/YOUR_USERNAME/ProgPOE-Part1-ST10275164-CyberAdvisor.git
cd ProgPOE-Part1-ST10275164-CyberAdvisor
```

2. Open the solution in Visual Studio (`ProgPOE-Part1-ST10275164-CyberAdvisor.sln`).

3. Install dependencies via NuGet:
   - `Figgle`

4. Build the project (Ctrl + Shift + B).

5. Run the project (F5 or Ctrl + F5).

---

## 🧪 GitHub Actions - CI

A CI workflow is configured in `.github/workflows/main.yml`. It runs on each `push` or `pull_request` to:
- Check for syntax errors
- Validate a successful build

---

## 🗂 Project Structure

```
ProgPOE-Part1-ST10275164-CyberAdvisor/
├── Program.cs                # Main chatbot logic
├── Assets/
│   └── welcome.wav           # Welcome audio file
├── .github/workflows/
│   └── ci.yml                # GitHub Actions CI configuration
├── .gitignore
├── .gitattributes
└── README.md
ProgPOE-Part1-ST10275164-CyberAdvisor.sln
```

---

## 💬 Example Questions You Can Ask

- How do I create a secure password?
- What is phishing?
- What is malware?
- How to avoid social engineering?
- Explain firewalls.
- And more...

Just type `"exit"` to end the chat.

---

## 🤝 Contributing

1. Fork the repository
2. Create a new branch (`git checkout -b feature-branch`)
3. Commit your changes (`git commit -am 'Add feature'`)
4. Push to the branch (`git push origin feature-branch`)
5. Create a new Pull Request

---

## 🧑 Author

**Muhammed Saif Alexander**  
Student Number: ST10275164

---


(this readme file's structure was provided to me by chatgpt)
