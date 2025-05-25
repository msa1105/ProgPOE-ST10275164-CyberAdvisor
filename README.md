# 🧠 CyberAdvisor - Enhanced AI Cybersecurity Mentor

CyberAdvisor is an advanced console-based C# chatbot application that serves as an intelligent cybersecurity assistant. It features memory management, sentiment analysis, personalized responses, and adaptive learning capabilities to provide a sophisticated conversational experience.

---

## ✨ Key Features

### 🎭 **Enhanced User Experience**
- **ASCII Logo Display** using [Figgle](https://www.nuget.org/packages/Figgle/) with animated typing effects
- **Welcome Audio** greeting using `System.Media.SoundPlayer`
- **Personalized Interactions** with user name collection and memory
- **Colored Console Output** with sentiment-based styling
- **Typing Animation Effect** for realistic bot responses
- **Session Summary** with detailed interaction statistics

### 🧠 **Advanced Memory Management**
- **User Profile Building** - Remembers personal information, job details, age, devices, and services
- **Interest Tracking** - Learns and remembers user's cybersecurity interests
- **Conversation History** - Maintains timestamped interaction logs
- **Pattern Recognition** - Extracts information using regex patterns and keyword matching
- **Contextual Recall** - References previous conversations for continuity

### 😊 **Intelligent Sentiment Analysis**
- **Real-time Emotion Detection** - Identifies user sentiment (worried, frustrated, curious, happy, confident, overwhelmed)
- **Adaptive Responses** - Modifies tone and approach based on detected emotions
- **Visual Feedback** - Uses colors and emojis to reflect conversation mood
- **Empathetic Communication** - Provides appropriate emotional support

### 🎯 **Personalized Response System**
- **Dynamic Content Generation** - Tailors advice based on user's job, experience level, and interests
- **Varied Response Pool** - Multiple responses per topic to avoid repetition
- **Contextual Personalization** - Adapts explanations for beginners vs. experienced users
- **Device-Specific Advice** - Customizes recommendations based on user's devices (iPhone, Android, etc.)

### 🔧 **Advanced Programming Features**
- **Delegate Implementation** - Uses delegates for flexible response processing and memory operations
- **Modular Architecture** - Separated concerns with dedicated classes for different functionalities
- **Error Handling** - Comprehensive exception management with user-friendly error messages
- **Cross-Platform Audio** - Handles audio playback with platform detection

---

## 🏗️ Architecture

### Core Components

**`BotUser`** - User profile management
- Stores personal information, interests, and conversation history
- Tracks session data and interaction counts
- Provides methods for memory storage and retrieval

**`MemoryManager`** - Advanced memory processing
- Pattern-based information extraction using regex
- Personalized response generation
- Contextual conversation recall
- Delegate-based memory operations

**`SentimentAnalyzer`** - Emotion detection system
- Keyword-based sentiment classification
- Color and emoji mapping for visual feedback
- Multi-sentiment scoring for accuracy

**`ResponseManager`** - Intelligent response system
- Keyword-based response matching
- Sentiment-aware response modification
- Fallback handling with personalization
- Delegate support for advanced filtering

---

## 🛠 Technologies Used

- **C# (.NET 8.0)**
- **Visual Studio 2022**
- **Figgle** - ASCII font rendering
- **System.Media** - Audio playback
- **System.Windows.Extensions** - Enhanced Windows functionality
- **Regular Expressions** - Pattern matching and extraction
- **Delegates & Lambda Expressions** - Functional programming features

---

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2022
- .NET SDK 8.0 or newer
- Git (for version control)
- Windows OS (for audio features)

### Setup

1. Clone the repository:
```bash
git clone https://github.com/YOUR_USERNAME/ProgPOE-Part1-ST10275164-CyberAdvisor.git
cd ProgPOE-Part1-ST10275164-CyberAdvisor
```

2. Open the solution in Visual Studio (`ProgPOE-Part1-ST10275164-CyberAdvisor.sln`)

3. Install NuGet packages:
   - `Figgle`
   - `System.Windows.Extensions`

4. Build the project (Ctrl + Shift + B)

5. Run the project (F5 or Ctrl + F5)

---

## 🗂 Project Structure

```
ProgPOE-Part1-ST10275164-CyberAdvisor/
├── Program.cs                    # Main application logic and user interface
├── BotUser.cs                    # User profile and memory management
├── MemoryManager.cs              # Advanced memory processing and personalization
├── ResponseManager.cs            # Intelligent response generation system
├── SentimentAnalyzer.cs          # Emotion detection and sentiment analysis
├── Assets/
│   └── welcome.wav              # Welcome greeting audio file
├── .github/workflows/
│   └── ci.yml                   # GitHub Actions CI configuration
├── .gitignore
├── .gitattributes
└── README.md
ProgPOE-Part1-ST10275164-CyberAdvisor.sln
```

---

## 💬 Interactive Features

### 🔐 Cybersecurity Topics Covered
- **Password Security** - Strong password creation and management
- **Phishing Prevention** - Email and web-based attack recognition
- **Privacy Protection** - Data protection and privacy settings
- **Scam Awareness** - Common scam identification and prevention
- **Malware Defense** - Virus protection and safe computing practices
- **WiFi Security** - Secure networking and public WiFi safety
- **Two-Factor Authentication** - Multi-factor security setup

### 🗣️ Conversation Examples

**Personal Information Sharing:**
```
User: "My job is software developer and I use an iPhone"
Bot: "Given your background in software developer, you probably already know some of this, but here's a refresher: [security advice]"
```

**Sentiment-Aware Responses:**
```
User: "I'm really worried about online security"
Bot: "😟 I understand your concerns - cybersecurity can feel overwhelming, but you're taking the right steps by learning!"
```

**Memory Recall:**
```
User: "What do you remember about me?"
Bot: "Let me think... I remember you're interested in: passwords, phishing. Other details: job: developer, devices: iPhone. We've chatted 15 times in this session!"
```

---

## 🧪 GitHub Actions CI

Automated CI workflow configured in `.github/workflows/ci.yml`:
- **Build Validation** - Ensures successful compilation
- **Syntax Checking** - Validates C# code syntax
- **Dependency Resolution** - Verifies NuGet package installation

---

## 📊 Session Analytics

The application tracks and displays:
- **User Profile Data** - Name, job, interests, devices
- **Interaction Metrics** - Total conversations, session duration
- **Sentiment History** - Mood tracking throughout session
- **Learning Progress** - Topics explored and knowledge areas covered

---

## 🔮 Advanced Programming Concepts Demonstrated

- **Object-Oriented Design** - Proper encapsulation and separation of concerns
- **Delegate Pattern** - Functional programming with delegates and lambda expressions
- **Regular Expressions** - Complex pattern matching for information extraction
- **Generic Collections** - Efficient data structure usage
- **Exception Handling** - Robust error management
- **SOLID Principles** - Clean, maintainable code architecture

---

## 🚀 Future Enhancement Possibilities

- **Machine Learning Integration** - More sophisticated sentiment analysis
- **Database Persistence** - Long-term user memory storage
- **Multi-Language Support** - Internationalization capabilities
- **Voice Recognition** - Speech-to-text input processing
- **Web Interface** - Browser-based interaction option
- **API Integration** - Real-time cybersecurity threat feeds

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature-amazing-enhancement`)
3. Commit your changes (`git commit -am 'Add amazing enhancement'`)
4. Push to the branch (`git push origin feature-amazing-enhancement`)
5. Create a Pull Request

---

## 🧑‍💻 Author

**Muhammed Saif Alexander**  
Student Number: ST10275164  
*Enhanced AI Cybersecurity Mentor Project*

---

## 📝 License

This project is for educational purposes as part of programming coursework.

---

*This README documents a significantly enhanced version of the original chatbot, now featuring advanced AI-like capabilities including memory management, sentiment analysis, and personalized user interactions.*
