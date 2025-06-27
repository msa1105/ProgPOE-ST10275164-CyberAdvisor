## 🛡️ CyberAdvisor: Your Personal AI Security Mentor</h1>
  <p><strong>A sophisticated, context-aware conversational agent designed to make cybersecurity accessible and actionable.</strong></p>
</div>

| **Author**              | **Student ID** | **Module**                  |
| ----------------------- | -------------- | --------------------------- |
| Muhammed Saif Alexander | ST10275164     | Programming (PROG) - POE    |

---

## 📖 Introduction

CyberAdvisor is an intelligent desktop chatbot built with **C#** and ported to **WPF**. It serves as an interactive mentor for cybersecurity, designed to demystify complex security topics for the everyday user.

This project moves beyond the limitations of simple command-based bots. It simulates a natural, helpful conversation by understanding user intent, remembering personal context, managing security-related tasks, and actively engaging the user through interactive learning modules. It is a comprehensive demonstration of modern software design principles applied to create a robust and user-centric AI-driven application.

---

## ✨ Key Features

CyberAdvisor integrates a suite of powerful features to create a cohesive and intelligent user experience.

#### 🧠 Intelligent Conversation & NLU
The bot is powered by a custom-built **Natural Language Understanding (NLU)** engine that can:
-   **Recognize Intent:** Accurately determine if a user is asking a question, setting a reminder, or providing information.
-   **Extract Entities:** Pull key details from sentences, like topics (`phishing`, `VPNs`), dates (`tomorrow at 5pm`), and task descriptions.
-   **Handle Variation:** Understand different ways of phrasing a request thanks to robust, RegEx-based pattern matching.

#### 📝 Contextual Memory System
The bot remembers key details from your conversation to provide a personalized experience.
-   **Learns About You:** Passively extracts and stores information like your name, job, devices, and skill level.
-   **Tailors Responses:** Uses remembered details to provide more relevant advice (e.g., security tips specific to a user's device).
-   **Recalls Information:** A user can ask, **"What do you know about me?"**, to see everything the bot has learned in the session.

#### ✔️ Proactive Task Management
Stay on top of your security to-do list with a built-in assistant.
-   **Natural Language Reminders:** Set tasks fluidly, such as `"remind me to run a virus scan in 3 days"`.
-   **Clarification Dialogue:** If a due date is missing, the bot will proactively ask for one.
-   **Centralized Task List:** View all pending reminders, neatly sorted by date, with a simple command or through a dedicated window.

#### 🎓 Interactive Learning Quiz
Makes learning cybersecurity fundamentals an engaging, gamified experience.
-   **Randomized Questions:** The quiz pulls from a large bank of questions, ensuring high replayability.
-   **Instant, Explanatory Feedback:** After every answer, the bot explains *why* it was correct or incorrect, reinforcing learning.
-   **Full User Control:** Start, play, and stop the quiz at any time without disrupting the main conversation flow.

#### 🖥️ Polished & Dynamic UI
The user interface is designed to be intuitive and visually informative.
-   **Sentiment-Aware Design:** Chat bubbles change color and show emojis based on the message's context (e.g., suggestion, error, summary).
-   **Asynchronous Processing:** The UI remains responsive while the bot "thinks," with a live typing indicator providing visual feedback.
-   **Clear Session Tools:** Easy-to-access buttons for major features like starting a quiz, viewing tasks, or clearing the chat.

---

## 🛠️ Technical Architecture

The project is built with a strong emphasis on **Separation of Concerns**, ensuring the code is maintainable, scalable, and easy to debug. The UI (`MainWindow.xaml.cs`) acts as a controller, orchestrating calls to specialized backend components.

-   **`NluEngine.cs`**: The "brain" of the bot. Its sole responsibility is to take raw user input and transform it into a structured `DetectedIntent` object with entities.
-   **`MemoryManager.cs`**: Manages the `BotUser`'s session data. It handles the logic for storing personal information and generating personalized response snippets.
-   **`ResponseManager.cs`**: Acts as the bot's knowledge base, containing the pre-defined responses for cybersecurity topics and fallback messages.
-   **`SentimentAnalyzer.cs`**: A utility that maps response types to specific UI colors and emojis, decoupling presentation logic from core functionality.
-   **Data Models (`BotUser.cs`, `QuizQuestion.cs`, etc.)**: Plain C# objects that define the core data structures used throughout the application.

This modular design means any single component can be upgraded or replaced without requiring a rewrite of the entire system.

---

## 🚀 How to Run the Project

### System Requirements
-   Windows 10 or 11
-   .NET 7.0 SDK
-   Visual Studio 2022

### Steps to Run
1.  **Clone the Repository**
    ```bash
    git clone https://github.com/msa1105/ProgPOE-ST10275164-CyberAdvisor.git
    ```
2.  **Navigate to the Directory**
    ```bash
    cd ProgPOE-ST10275164-CyberAdvisor
    ```
3.  **Open the Solution**
    -   Locate and open the `ProgPOE-Part1-ST10275164-CyberAdvisor.sln` file with Visual Studio 2022.
4.  **Restore Dependencies**
    -   Visual Studio should automatically restore the required NuGet packages. If not, right-click the solution in the Solution Explorer and select "Restore NuGet Packages."
5.  **Build and Run**
    -   Press `F5` or click the "Start" button in the Visual Studio toolbar to compile and launch the application.
