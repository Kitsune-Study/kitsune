﻿using Corelibs.Basic.Blocks;
using Mediator;

namespace Manabu.UseCases.Content.Phrases;

public record GetPhraseQuery(
    string PhraseId) : IQuery<Result<GetPhraseQueryResponse>>;

public record GetPhraseQueryResponse(PhraseDetailsDTO Content);

public record PhraseDetailsDTO(
    string Id,
    string Original,
    string[] Translations,
    string[] Contexts,
    bool Learned,
    AudioDTO[] Audios,
    WordMeaningDTO[]? WordMeanings = null);

public record AudioDTO(string Id, string Href);

public record WordMeaningDTO(
    string Id,
    string DictionaryForm,
    string OriginalWriting,
    string TargetWriting,
    string Translation,
    string? WordInflectionId = null,
    string? Reading = null,
    string? SelectedReading = null,
    string? WritingMode = null,
    string? CustomWriting = null);
