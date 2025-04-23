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

        PlayWelcomeAudio();      //this plays the welcome audio

        DisplayAsciiLogo();      //this shows the ASCII art

        AskForUserName();        //this asks you for your name

        // this initiates and controls the chat loop
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n{currentUser.Name}, ask me anything (type 'exit' to quit): ");
            Console.ResetColor();
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "exit")
            {
                Console.WriteLine($"\n👋 Goodbye, {currentUser.Name}! Stay cyber safe.");
                break;
            }

            RespondToUser(userInput);
        }
    }

    //plays .wav greeting using SoundPlayer

    static void PlayWelcomeAudio()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("C:\\Users\\alexa\\source\\repos\\ProgPOE-Part1-ST10275164-CyberAdvisor\\ProgPOE-Part1-ST10275164-CyberAdvisor\\Assets\\welcome.wav"))
                {
                    player.Load();    // Preloads audio
                    player.PlaySync(); // Plays audio synchronously
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


    //this is the function that shows the ASCII logo using Figgle
    static void DisplayAsciiLogo()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(FiggleFonts.Slant.Render("CyberAdvisor"));
        Console.ResetColor();
    }

    //this function asks for the user's name and stores it (botuser.cs)
    static void AskForUserName()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("🤖 What's your name? ");
        Console.ResetColor();

        string nameInput = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(nameInput))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("⚠️  Please enter a valid name: ");
            Console.ResetColor();
            nameInput = Console.ReadLine();
        }

        currentUser.Name = nameInput.Trim();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"🎉 Welcome, {currentUser.Name}! I'm here to help you stay cyber safe.\n");
        Console.ResetColor();
    }

    // this allows the bot to respond based on user inputs using if.contains statements
    static void RespondToUser(string input)
    {
        input = input.ToLower().Trim();

        Console.ForegroundColor = ConsoleColor.White;

        if (input.Contains("how you") || input.Contains("are you"))
        {
            Console.WriteLine($"I'm doing great, {currentUser.Name}! Thanks for asking.");
        }
        else if (input.Contains("do you") || input.Contains("how do you") || input.Contains("purpose"))
        {
            Console.WriteLine($"I'm here to raise your cybersecurity awareness, {currentUser.Name}.");
        }
        else if (input.Contains("ask you") || input.Contains("ask"))
        {
            Console.WriteLine("You can ask me about password safety, phishing, safe browsing, and more.");
        }
        else if (input.Contains("phishing"))
        {
            Console.WriteLine("Phishing is a cyber attack where hackers trick you into giving sensitive info by pretending to be someone you trust.");
        }
        else if (input.Contains("strong password"))
        {
            Console.WriteLine("Use a mix of upper/lowercase letters, numbers, and symbols. Avoid dictionary words.");
        }
        else if (input.Contains("safe browsing"))
        {
            Console.WriteLine("Safe browsing means using secure sites (HTTPS), avoiding suspicious links, and keeping your browser updated.");
        }
        else if (input.Contains("unknown email") || input.Contains("click") && input.Contains("email"))
        {
            Console.WriteLine("⚠️ Never click links in emails from unknown senders. They might lead to phishing or malware.");
        }
        else if (input.Contains("malware") || input.Contains("virus"))
        {
            Console.WriteLine("Malware or Viruses are software designed to harm your device or steal your data.");
        }
        else if (input.Contains("detect phishing") || input.Contains("phishing"))
        {
            Console.WriteLine("Phishing is the use of fake information, usually through emails to gain access to sensitive information. To detect phishing look for bad grammar, fake URLs, urgent language, and mismatched email addresses.");
        }
        else if (input.Contains("public wifi") || input.Contains("wifi") || input.Contains("wifi safe"))
        {
            Console.WriteLine("Wi-Fi isn't as safe as many people think. Use a VPN or avoid logging into sensitive accounts on public Wi-Fi.");
        }
        else if (input.Contains("vpn"))
        {
            Console.WriteLine("A VPN encrypts your internet connection, protecting your data on public networks.");
        }
        else if (input.Contains("share passwords"))
        {
            Console.WriteLine("Nope! Passwords should always be kept private, even from friends.");
        }
        else if (input.Contains("two factor") || input.Contains("2fa"))
        {
            Console.WriteLine("Two Factor Authentication is a second layer of login security using your phone or email to verify your identity.");
        }
        else if (input.Contains("apps steal") || input.Contains("app permissions"))
        {
            Console.WriteLine("Yes, always check app permissions and install from trusted sources.");
        }
        else if (input.Contains("secure my phone") || input.Contains("phone"))
        {
            Console.WriteLine("To make sure your phone is secure, use a strong passcode, biometric lock, and keep software up to date.");
        }
        else if (input.Contains("update my software"))
        {
            Console.WriteLine("Updates patch security holes hackers can use to attack you.");
        }
        else if (input.Contains("reuse passwords"))
        {
            Console.WriteLine("It's risky. If one account gets hacked, others using the same password are vulnerable.");
        }
        else if (input.Contains("report cybercrime"))
        {
            Console.WriteLine("Contact your country's cybercrime unit or police department's fraud division.");
        }
        else if (input.Contains("help") || input.Contains("options"))
        {
            Console.WriteLine("Try asking about: phishing, safe passwords, safe browsing, 2FA, malware, or VPNs.");

        }
        else if (input.Contains("thank"))
        {
            Console.WriteLine($"You're welcome {currentUser.Name}, I'm here to help. You can type 'quit' in the console to close the app.");
        }
        else if (input.Contains("exit") || input.Contains("quit") || input.Contains("close"))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nGoodbye, {currentUser.Name}! Stay safe online.");
            Console.ResetColor();
            Environment.Exit(0);
        }
        else  // this is to respond to misinputs and keep the flow of the conversation
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Hmm... I didn’t understand that, {currentUser.Name}. Try asking about phishing, passwords, or safe browsing.");
        }

        Console.ResetColor();
    }

}

