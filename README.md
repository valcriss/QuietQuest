# QuietQuest

**A volume-monitoring and penalty system for noisy gamers**

## Background

My son loves playing video games in his room, but sometimes he shouts so loudly into his headset that it wakes up the neighbors (and me!). I wanted a way to gently remind him to keep his voice down without storming in to pause his game. QuietQuest was born as a fun, automated "coach": if he exceeds a configured volume threshold, the system issues a warning and triggers a penalty effect for a short time.

## Features

- **Real‑time volume monitoring** via microphone input — keep an ear on every decibel so sneaky shouts don't escape notice 👂🔍
- **Configurable threshold**: set the decibel level at which to trigger penalties — like a sonic speed camera: flash too loud, ticket incoming 🚨🎶
- **Penalties** (randomly selected from a list, because who doesn’t love a surprise twist?):
  - Invert keyboard keys or introduce input lag — watch his WASD melt into QZSD mayhem ⌨️😂
  - Mouse drift (subtle random cursor movement) — turn precise sniping into cursor salsa 💃🖱️
  - Play annoying background music or random notification sounds — elevator hits or phantom pings guaranteed to disrupt 🛗📳
  - Echo his own voice with a slight delay — let him argue with his own echo for 10 seconds 🗣️🪞
  - Mute system audio or microphone (re‑mute if he tries to unmute) — silence is golden, and enforced 🔇🥇
  - Display a semi‑transparent overlay with an irritating GIF — unstoppable dancing cat or blinking meme to steal his focus 🐈‍⬛✨
  - Temporarily change screen brightness — from blackout night to neon blinding, your call 🌒☀️
  - Deliver troll voice messages with humorous advice — "Try whispering next time" meets dad jokes on demand 🎙️😈
- **Agent–Admin architecture**: two apps in perfect harmony — the agent in your son's session applies the fun effects, while the admin client on your PC pulls the strings like a puppet master 🕹️👔
- **Remote administration**: micromanage his volume from anywhere on your local network — because who needs parental prowess when you have Wi-Fi? 📶🛋️
- **Secure deployment**: installed with admin rights and locked down tighter than Fort Knox — he won't even know where to begin to disable it 🔐🏰

## Architecture Overview

```
+---------------------+      HTTP       +--------------------+
| QuietQuestAdmin     |  <----------   | QuietQuestAgent    |
| (Admin client on    |      API       | (Agent in user     |
|  your PC)           |                |  session)          |
+---------------------+                  +--------------------+
```

- **QuietQuestAgent**: runs in the user session on your son's PC, monitors microphone volume and applies penalties (audio, visual, input effects).
- **QuietQuestAdmin**: WPF application running on the administrator's PC, connects to the agent's HTTP API to view status, adjust configuration, and trigger penalties.

## Installation

### Prerequisites

- Windows 10/11 (x64)
- .NET 8 Runtime installed
- Administrator privileges to install the agent

### Install the Agent

1. Run `QuietQuestAgentInstaller.exe` **as Administrator**.
2. The agent will install to `C:/Program Files/QuietQuestAgent` and auto‑start on user login.

### Launch the Admin Client

- On your PC, open **QuietQuestAdmin.exe** from the Start Menu.
- Enter your son's PC IP (e.g., `http://192.168.x.y:5005`) and click **Connect**.

## Configuration

- **Threshold**: incoming decibel level (0–100)
- **Active**: enable or disable monitoring without reinstalling
- **Penalty Duration**: default 10 seconds (configurable in code or future API)

## Usage

1. Connect Admin Client to the service
2. Toggle **Active** to start/stop monitoring
3. Adjust **Threshold** as needed
4. Watch **Last Penalty** and **Running** status in real time
5. Click **Trigger Penalty** to test effects

## Customization

- Add or remove `IPenalty` implementations in the service code
- Swap audio files or GIFs by replacing assets in the `Assets` folder
- Modify the Inno Setup scripts to customize installer UI or paths

## Troubleshooting

- **Service fails to start**: run `QuietQuest.ClientService.exe --console` to view errors
- **Agent overlay not showing**: ensure agent is running in the user session and not blocked by antivirus
- **Permission issues**: verify both installer runs as Administrator and service is set to LocalSystem

## Contributing

Feel free to submit issues or pull requests on the [GitHub repository](https://github.com/yourusername/QuietQuest). Suggestions for new penalties and improvements are welcome!

## License

MIT License © 2025  

