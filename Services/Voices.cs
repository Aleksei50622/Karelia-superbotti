using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbotti.Services
{
    public class Voices(LocalAIClient localai, IConfiguration config)
    {
        public string Voice { get; set; } =
        config["LocalAI:DefaultVoice"] is string voice && 
            !string.IsNullOrWhiteSpace(voice) && 
            voice != "DEFAULT_VOICE" 
                ? voice 
                : "";
    }
}
