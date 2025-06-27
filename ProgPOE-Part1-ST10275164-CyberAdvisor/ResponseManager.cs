// --- ResponseManager.cs (Complete Grandmaster Knowledge Base Version) ---

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class TopicData
    {
        public List<string> Responses { get; set; }
    }

    public class ResponseManager
    {
        private readonly Dictionary<string, TopicData> topicResponses;
        private readonly Random random;
        private readonly List<string> fallbackResponses;

        public ResponseManager()
        {
            random = new Random();
            topicResponses = new Dictionary<string, TopicData>();
            fallbackResponses = new List<string>();
            InitializeTopicResponses();
            InitializeFallbackResponses();
        }

        private void InitializeTopicResponses()
        {
            // --- Authentication Family ---
            topicResponses["Password"] = new TopicData
            {
                Responses = new List<string> {
                "A strong password is your first line of defense. Aim for at least 16 characters with a mix of uppercase, lowercase, numbers, and symbols like !@#$%.",
                "Avoid using personal information like birthdays, names, or pet's names. This info is often public and easy for attackers to guess.",
                "Passphrases are a modern, highly secure method. A random four-word phrase like 'CorrectHorseBatteryStaple' is extremely hard to crack but easy to remember.",
                "Using a password manager is highly recommended. It generates and stores unique, complex passwords for every site, so you only have to remember one strong master password.",
                "Never write your passwords on a sticky note attached to your monitor or keep them in an unencrypted text file. Treat them like a house key.",
                "Regularly check if your email address has been involved in data breaches using 'Have I Been Pwned'. If so, change the password for that service and any others that shared it."
            }
            };
            topicResponses["TwoFactorAuth"] = new TopicData
            {
                Responses = new List<string> {
                "Two-Factor Authentication (2FA) is a critical security layer. It combines something you know (password) with something you have (phone).",
                "Enable 2FA on every important account: email, banking, social media. It's one of the single most effective things you can do to protect your digital life.",
                "Use an authenticator app (like Google Authenticator, Authy) for 2FA instead of SMS texts. SMS can be vulnerable to 'SIM-swapping' attacks.",
                "When you set up 2FA, you'll get backup codes. Print them and store them somewhere safe, like a locked drawer. They are your lifeline if you lose your phone.",
                "A hardware security key (like a YubiKey) is the gold standard for 2FA. It's a physical device that you plug in or tap to approve a login, making it immune to phishing.",
                "Even with 2FA, be cautious of 'MFA fatigue' attacks, where an attacker spams you with login requests hoping you'll accidentally approve one."
            }
            };

            // --- Malware Family ---
            topicResponses["Malware"] = new TopicData
            {
                Responses = new List<string> {
                "Malware is short for 'Malicious Software'—an umbrella term for viruses, trojans, ransomware, and spyware.",
                "The best defense against malware is caution. Don't click suspicious links, don't open unexpected attachments, and only download software from official sources.",
                "Keep your operating system and all applications (especially your web browser) updated. Updates contain critical security patches that block malware.",
                "Use a reputable antivirus program and ensure its real-time protection is enabled. It's your digital immune system.",
                "If you suspect malware, disconnect the device from the internet to prevent it from spreading. Then, run a full system scan with your antivirus."
            }
            };
            topicResponses["Ransomware"] = new TopicData
            {
                Responses = new List<string> {
                "Ransomware is malware that encrypts your personal files—documents, photos, videos—making them completely inaccessible.",
                "After encrypting files, ransomware displays a message demanding a payment (a 'ransom'), usually in cryptocurrency, for the decryption key.",
                "The single most important defense is having regular, recent backups of your important data, stored offline or in the cloud where malware can't reach it.",
                "Security experts and law enforcement universally advise against paying the ransom. There's no guarantee you'll get your files back, and it funds criminal enterprises.",
                "Ransomware often spreads through phishing emails with malicious attachments, or by exploiting unpatched security vulnerabilities in software."
            }
            };
            topicResponses["Virus"] = new TopicData
            {
                Responses = new List<string> {
                "A computer virus is malware that, when executed, replicates by inserting its own code into other programs. It needs a host program to spread.",
                "Viruses spread when the software they are attached to is transferred between computers via network, disk, or infected email attachments.",
                "Effects can range from annoying pop-ups to destroying data, corrupting your system, or stealing information.",
                "A good antivirus program is essential for detecting and removing viruses by scanning files against a database of known virus 'signatures'.",
                "To avoid viruses, be very cautious about opening unexpected email attachments, even from people you know, and only download from trusted, official sources."
            }
            };

            // --- Network & Web Family ---
            topicResponses["VPN"] = new TopicData
            {
                Responses = new List<string> {
                "A VPN (Virtual Private Network) encrypts your internet traffic, making it unreadable to anyone on your network, including on public Wi-Fi or by your ISP.",
                "A VPN hides your real IP address, which helps protect your privacy and can allow you to access content that might be restricted in your geographical region.",
                "Be very careful with 'free' VPN services. They often have slow speeds, data limits, or in the worst cases, may sell your browsing data to advertisers.",
                "For maximum privacy, choose a paid VPN provider with a strict 'no-logs' policy that has undergone public, third-party security audits.",
                "A VPN does not make you 100% anonymous. It's one powerful tool in a privacy toolkit, but doesn't protect you from malware or if you voluntarily give your data to a website like Facebook.",
                "Using a VPN can sometimes slow your connection, as data travels through an extra server. Reputable providers minimize this speed loss."
            }
            };
            topicResponses["WiFiSecurity"] = new TopicData
            {
                Responses = new List<string> {
                "When setting up home Wi-Fi, always change the default administrator username and password for the router's settings page.",
                "Your Wi-Fi network should be protected with a strong, unique password using WPA3 or at least WPA2 encryption.",
                "Using a 'Guest Network' on your home router is great practice. It provides internet for visitors on an isolated network, so they can't access your personal devices or files.",
                "Be extremely careful on public Wi-Fi (cafes, airports). An attacker on the same network can 'sniff' your traffic. Always use a VPN on public networks.",
                "Your router's firmware should be kept up to date. Manufacturers release patches for security holes. Check your router manufacturer's website for updates."
            }
            };
            topicResponses["HTTPS"] = new TopicData
            {
                Responses = new List<string> {
                "HTTPS (Hypertext Transfer Protocol Secure) means the connection between your browser and the website is encrypted. Look for the padlock icon in the address bar.",
                "Encryption provided by HTTPS prevents 'man-in-the-middle' attacks, where an attacker tries to eavesdrop on your communication.",
                "Always look for the HTTPS padlock before entering any sensitive information like a password or credit card number.",
                "HTTPS uses SSL/TLS technology to create the secure connection, ensuring data integrity, confidentiality, and authentication.",
                "Modern browsers will warn you loudly if you're on an insecure HTTP site where you're about to enter a password. Heed these warnings!"
            }
            };

            // --- Social Engineering Family ---
            topicResponses["Phishing"] = new TopicData
            {
                Responses = new List<string> {
                "Phishing attacks use fake emails, texts, or websites to trick you into revealing sensitive information. They often impersonate trusted brands like Microsoft, Google, or your bank.",
                "Phishing attacks create a false sense of urgency. They use phrases like 'Your account will be suspended' to make you panic and click without thinking.",
                "Always inspect the sender's email address. Scammers often use addresses that look close to a real one, like 'support@microsft.com'.",
                "Before clicking any link in an email, hover your mouse over it. The actual destination URL will pop up. If it looks suspicious, don't click it.",
                "Be wary of emails with poor grammar or spelling mistakes. Legitimate companies usually have teams that proofread their communications.",
                "'Smishing' is phishing via SMS (text messages), and 'Vishing' is phishing via voice calls. Be suspicious of urgent requests from any channel."
            }
            };

            // --- Concepts & Practices Family ---
            topicResponses["DataBreach"] = new TopicData
            {
                Responses = new List<string> {
                "A data breach is an incident where sensitive information is stolen or released from a company's database by an unauthorized individual.",
                "The stolen data often includes usernames, email addresses, and passwords, which criminals then sell on the dark web or use for identity theft.",
                "The best defense against the impact of a breach is to use unique passwords for every service. That way, a breach at one company doesn't compromise your accounts elsewhere.",
                "You can check if your email account has been compromised in known data breaches using the free service 'Have I Been Pwned'.",
                "If you find out you've been part of a breach, immediately change the password for that service and enable 2FA if you haven't already."
            }
            };
            topicResponses["Encryption"] = new TopicData
            {
                Responses = new List<string> {
                "Encryption is the process of scrambling data into a code (ciphertext) to prevent unauthorized access. Only someone with the correct key can unscramble it.",
                "End-to-end encryption (E2EE), used by apps like Signal and WhatsApp, ensures that only you and the recipient can read what is sent. No one in between, not even the company, can access it.",
                "Full-disk encryption, like BitLocker on Windows and FileVault on macOS, encrypts your entire hard drive. If your laptop is stolen, the thief can't access your files without your password.",
                "HTTPS is encryption 'in transit', protecting your data as it travels across the internet from your browser to a website.",
                "Encryption is a fundamental building block of digital security, protecting everything from online banking to private messages."
            }
            };
        }

        private void InitializeFallbackResponses()
        {
            fallbackResponses.Add("I'm not quite sure about that. Could you rephrase or ask about another cybersecurity topic?");
            fallbackResponses.Add("That's a bit outside my current knowledge base. I'm great with topics like passwords, malware, and VPNs though!");
        }

        public string GetKeywordResponse(string topic, BotUser user)
        {
            if (topicResponses.ContainsKey(topic))
            {
                var topicData = topicResponses[topic];
                return topicData.Responses[random.Next(topicData.Responses.Count)];
            }
            return null;
        }

        public string GetFallbackResponse(BotUser user)
        {
            return fallbackResponses[random.Next(fallbackResponses.Count)];
        }
    }
}