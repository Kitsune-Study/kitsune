﻿using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Manabu.Entities.Content.Lessons;
using Manabu.Entities.Content.Phrases;
using Manabu.Entities.Content.Users;

namespace Manabu.Entities.Content.Conversations;

public class Conversation : Entity<ConversationId>, IAggregateRoot<ConversationId>
{
    public static string DefaultCollectionName { get; } = "conversations";

    public UserId Owner { get; private set; }
    public List<PhraseData> Phrases { get; private set; }
    public List<LessonId> Lessons { get; private set; }
    public bool IsArchived { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Conversation(
        string name,
        LessonId lessonId,
        UserId owner)
    {
        Name = name;
        Lessons = new() { lessonId };
        Owner = owner;
    }

    public void AddPhrase(
        PhraseId phrase, 
        int index = int.MaxValue)
    {
        Phrases ??= new();
        Phrases.InsertClamped(new("", "", phrase), index);
    }
    
    public bool RemoveFromLesson(LessonId lesson) =>
        Lessons is null ? false :
        Lessons.Count <= 1 ? true : Lessons.Remove(lesson);

    public bool ChangeSpeaker(string speaker, PhraseId phrase, int index = -1)
    {
        var phraseData = Phrases.Get(p => p.Phrase == phrase, index, out var foundIndex);
        if (phraseData is null || speaker is null || speaker == phraseData.Speaker)
            return false;
        
        Phrases[foundIndex] = phraseData with { Speaker = speaker };
        return true;
    }

    public bool ChangeSpeakerTranslation(string speakerTranslation, PhraseId phrase, int index = -1)
    {
        var phraseData = Phrases.Get(p => p.Phrase == phrase, index, out var foundIndex);
        if (phraseData is null || speakerTranslation is null || speakerTranslation == phraseData.SpeakerTranslation)
            return false;

        Phrases[foundIndex] = phraseData with { SpeakerTranslation = speakerTranslation };
        return true;
    }

    public bool MovePhrases(IEnumerable<PhraseId> phrases, Conversation newConversation, int index = int.MaxValue)
    {
        foreach (var phrase in phrases)
        {
            if (!Phrases.RemoveIf(p => p.Phrase == phrase))
                return false;

            newConversation.AddPhrase(phrase, index);
        }

        return true;
    }

    public bool ReorderPhrase(PhraseId phrase, int index)
    {
        var phraseData = Phrases.FirstOrDefault(p => p.Phrase == phrase);
        if (phraseData is null) 
            return false;

        if (!Phrases.Remove(phraseData))
            return false;

        Phrases.InsertClamped(phraseData, index);

        return true;
    }

    public bool HasContent() =>
        !Phrases.IsNullOrEmpty();
      

    public record PhraseData(string Speaker, string SpeakerTranslation, PhraseId Phrase);
}

public class ConversationId : EntityId { public ConversationId(string value) : base(value) {} }
