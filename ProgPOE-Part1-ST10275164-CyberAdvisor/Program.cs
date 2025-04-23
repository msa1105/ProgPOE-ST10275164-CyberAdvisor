using System;
using System.Media;
using System.Threading;
using Figgle;
using ProgPOE_Part1_ST10275164_CyberAdvisor;

class Program
{
    static BotUser currentUser = new BotUser();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        PlayWelcomeAudio();      //plays welcome audio
        DisplayAsciiLogo();      //displays the ASCII art title
        AskForUserName();        //asks for users name

        //this starts the chatbot loop
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n{currentUser.Name}, ask me anything (type 'exit' to quit): ");
            Console.ResetColor();

            string userInput = Console.ReadLine();

            if (userInput?.ToLower() == "exit")
            {
                Console.WriteLine($"\n👋 Goodbye, {currentUser.Name}! Stay cyber safe.");
                break;
            }

            RespondToUser(userInput);
        }
    }

    //plays welcome greeting audio
    static void PlayWelcomeAudio()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("C:\\Users\\alexa\\source\\repos\\ProgPOE-Part1-ST10275164-CyberAdvisor\\ProgPOE-Part1-ST10275164-CyberAdvisor\\Assets\\welcome.wav"))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing audio: " + ex.Message);
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Audio playback is not supported on this platform.");
            Console.ResetColor();
        }
    }

    //this displays the ASCII logo
    static void DisplayAsciiLogo()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(FiggleFonts.Slant.Render("CyberAdvisor"));
        Console.ResetColor();
        Thread.Sleep(100); // slight pause
    }

    //this asks the users name
    static void AskForUserName()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("🤖 What's your name? ");
        Console.ResetColor();

        string nameInput = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(nameInput))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Please enter a valid name: ");
            Console.ResetColor();
            nameInput = Console.ReadLine();
        }

        currentUser.Name = nameInput.Trim();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nWelcome, {currentUser.Name}! I'm here to help you stay cyber safe.\n");
        Console.ResetColor();
    }

    //responses to user input
    static void RespondToUser(string input)
    {
        input = input?.ToLower().Trim();

        void BotReply(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(10); // typing effect
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        if (input.Contains("how you") || input.Contains("are you"))
        {
            BotReply($"I'm doing great, {currentUser.Name}! Thanks for asking.");
        }
        else if (input.Contains("do you") || input.Contains("how do you") || input.Contains("purpose"))
        {
            BotReply($"I'm here to raise your cybersecurity awareness, {currentUser.Name}.");
        }
        else if (input.Contains("ask you") || input.Contains("ask"))
        {
            BotReply("You can ask me about password safety, phishing, safe browsing, and more.");
        }
        else if (input.Contains("phishing"))
        {
            BotReply("Phishing is a cyber attack where hackers trick you into giving sensitive info by pretending to be someone you trust.");
        }
        else if (input.Contains("strong password") || input.Contains("password safety") || input.Contains("password"))
        {
            BotReply("A strong password is at least 12 characters long, includes letters, numbers, and symbols, and is unique to each account.");
        }
        else if (input.Contains("create strong password") || input.Contains("password tips"))
        {
            BotReply("Use a mix of upper/lowercase letters, numbers, and symbols. Avoid dictionary words.");
        }
        else if (input.Contains("safe browsing") || input.Contains("browsing"))
        {
            BotReply("Safe browsing means using secure sites (HTTPS), avoiding suspicious links, and keeping your browser updated.");
        }
        else if (input.Contains("unknown email") || (input.Contains("click") && input.Contains("email")))
        {
            BotReply("Never click links in emails from unknown senders. They might lead to phishing or malware.");
        }
        else if (input.Contains("malware") || input.Contains("virus"))
        {
            BotReply("Malware or Viruses are software designed to harm your device or steal your data.");
        }
        else if (input.Contains("detect phishing") || input.Contains("prevent phishing"))
        {
            BotReply("Phishing is the use of fake information, usually through emails to gain access to sensitive information. To detect phishing look for bad grammar, fake URLs, urgent language, and mismatched email addresses.");
        }
        else if (input.Contains("public wifi") || input.Contains("wifi") || input.Contains("wifi safe"))
        {
            BotReply("Wi-Fi isn't as safe as many people think. Use a VPN or avoid logging into sensitive accounts on public Wi-Fi.");
        }
        else if (input.Contains("vpn"))
        {
            BotReply("A VPN encrypts your internet connection, protecting your data on public networks.");
        }
        else if (input.Contains("share password") || input.Contains("sharing") || input.Contains("friends"))
        {
            BotReply("It's not safe to share passwords, even with friends. Use a password manager instead.");
        }
        else if (input.Contains("password with") || input.Contains("friends") || input.Contains("password friend"))
        {
            BotReply("Nope! Passwords should always be kept private, even from friends.");
        }
        else if (input.Contains("two factor") || input.Contains("2fa"))
        {
            BotReply("Two Factor Authentication is a second layer of login security using your phone or email to verify your identity.");
        }
        else if (input.Contains("apps steal") || input.Contains("app permissions"))
        {
            BotReply("Yes, always check app permissions and install from trusted sources.");
        }
        else if (input.Contains("secure my phone") || input.Contains("phone secure"))
        {
            BotReply("To make sure your phone is secure, use a strong passcode, biometric lock, and keep software up to date.");
        }
        else if (input.Contains("update software") || input.Contains("software update") || input.Contains("update"))
        {
            BotReply("Updates patch security holes hackers can use to attack you. Be sure to always update your software for the latest security patches.");
        }
        else if (input.Contains("reuse password") || input.Contains("same password"))
        {
            BotReply("It's not safe to reuse passwords across multiple accounts. Use a password manager to keep track of them.");
        }
        else if (input.Contains("password manager") || input.Contains("password manager safe"))
        {
            BotReply("Lots of password managers are safe and help you create and store strong passwords securely, however data leaks still leave you at risk.");
        }
        else if (input.Contains("password manager risky") || input.Contains("password manager risk"))
        {
            BotReply("It's risky. If one account gets hacked, others using the same password are vulnerable.");
        }
        else if (input.Contains("report cybercrime") || input.Contains("report scam") || input.Contains("report fraud"))
        {
            BotReply("You can report cybercrime to your local authorities or a national cybercrime unit.");
        }
        else if (input.Contains("scam") || input.Contains("fraud") || input.Contains("hack") || input.Contains("hacked"))
        {
            BotReply("Contact your country's cybercrime unit or police department's fraud division.");
        }
        else if (input.Contains("help") || input.Contains("options"))
        {
            BotReply("Try asking about: phishing, safe passwords, safe browsing, 2FA, malware, or VPNs.");
        }
        else if (input.Contains("thank"))
        {
            BotReply($"You're welcome {currentUser.Name}, I'm here to help. You can type 'quit' in the console to close the app.");
        }
        else if (input.Contains("bye") || input.Contains("see you") || input.Contains("later"))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            BotReply($"\nGoodbye, {currentUser.Name}! Stay safe online.");
            Console.ResetColor();
            Environment.Exit(0);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            BotReply($"Hmm... I'm sorry, I didn’t understand that, {currentUser.Name}. Try asking about phishing, passwords, or safe browsing.");
            Console.ResetColor();
        }
    }
}