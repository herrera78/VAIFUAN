using Havir.Sockets.Entities;
using System;
using System.Collections;
using System.Collections.Generic;



namespace Havir.Api.Speech
{

    public delegate void KeyWordRecognized(KeywordRecognizedArgs args);
    public delegate void WilcardRecognized(WilcardRecognizedArgs args);
    public delegate void EmitMessage(UnityActionMessage message); 
    public interface IVoiceRecognizer : IDisposable
    {

        event KeyWordRecognized OnKeywordRecognized;
        event WilcardRecognized OnWilcardRecognized;

        /// <summary>
        /// Inicializa al reconocedor
        /// </summary>
        /// <param name="dictation">Activar proceso de dictación requerido</param>
        /// <param name="keywordRecognation">Activar proceso de reconocimiento de palabras clave</param>
        void InitRecognizer(bool dictation, bool keywordRecognation);


        Guid AddSemanticRecognition(string semanticKey, string[] keywords);

        /// <summary>
        /// Permite agregar un grupo de palabras clave al reconocedor de voz
        /// </summary>
        /// <param name="keywords">Grupo de palabras clave que serán agregadas al reconocedor de voz</param>
        /// <param name="startInmmediately">Booleano que permite activar el reconocedor de voz al momento de agregar las palabras clave.
        /// Se debe usar preferiblemente luego del InitRecognizer
        /// </param>
        /// <returns>Identificador único del grupo creado en el reconocedor de voz</returns>
        Guid AddKeywordRecognition(string[] keywords);

        ///// <summary>
        ///// Permite agregar un grupo de palabras clave al reconocedor de voz
        ///// </summary>
        ///// <param name="keywords">Grupo de palabras clave que serán agregadas al reconocedor de voz</param>
        ///// <param name="startInmmediately">Booleano que permite activar el reconocedor de voz al momento de agregar las palabras clave.
        ///// Se debe usar preferiblemente luego del InitRecognizer
        ///// </param>
        ///// <returns>Identificador único del grupo creado en el reconocedor de voz</returns>
        //Guid AddKeywordRecognition(string[] keywords, Action<string> action, bool startInmmediately = false);

        /// <summary>
        /// Elimina un grupo de palabras claves del reconocedor de voz
        /// </summary>
        /// <param name="id">Identificador único del grupo de palabras clave</param>
        void RemoveKeywordRecognition(Guid id);

        /// <summary>
        /// Elimina un grupo de palabras claves del reconocedor de voz
        /// </summary>
        /// <param name="id">Identificador único del grupo de palabras clave</param>
        void StopKeywordRecognition(Guid id);

        void StartKeywordRecognition(Guid id);

        /// <summary>
        /// Elimina un grupo de palabras claves del reconocedor de voz
        /// </summary>
        /// <param name="id">Identificador único del grupo de palabras clave</param>
        void PauseKeywordRecognition();

        Guid AddWildcardGrammar(string semmanticKey, string[] keywords);
        // Consider renaming it? What is this doing?
    }

    public class KeywordRecognizedArgs
    {

        public string SemanticKey { get; set; }
        public string Text { get; set; }
        public KeywordRecognizedArgs(string semanticKey, string text)
        {
            SemanticKey = semanticKey;
            Text = text;
        }
    }

    // Is this dictation? Just run dictation in parse for the keyword.
    public class WilcardRecognizedArgs : KeywordRecognizedArgs
    {
        public string KeyWord { get; set; }

        public WilcardRecognizedArgs(string semanticKey, string keyword, string text) : base(semanticKey, text)
        {
            SemanticKey = semanticKey;
            Text = text;
            KeyWord = keyword;
        }
    }
}