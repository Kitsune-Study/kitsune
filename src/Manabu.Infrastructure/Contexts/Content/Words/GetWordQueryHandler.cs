﻿using Corelibs.Basic.Blocks;
using Corelibs.Basic.Collections;
using Corelibs.MongoDB;
using Manabu.Entities.Content.WordMeanings;
using Manabu.Entities.Content.Words;
using Manabu.UseCases.Content.Words;
using Mediator;
using MongoDB.Driver;
using static Manabu.Infrastructure.CQRS.Content.Words.GetWordsQueryHandler;

namespace Manabu.Infrastructure.CQRS.Content.Words;

public class GetWordQueryHandler : IQueryHandler<GetWordQuery, Result<GetWordQueryResponse>>
{
    private readonly MongoConnection _mongoConnection;

    public GetWordQueryHandler(
        MongoConnection mongoConnection)
    {
        _mongoConnection = mongoConnection;
    }

    public async ValueTask<Result<GetWordQueryResponse>> Handle(GetWordQuery query, CancellationToken cancellationToken)
    {
        var result = Result<GetWordQueryResponse>.Success();

        var wordsCollection = _mongoConnection.Database.GetCollection<Word>(Word.DefaultCollectionName);
        var wordFilter = Builders<Word>.Filter.Eq(x => x.Id, new WordId(query.WordId));
        var wordProjection = Builders<Word>.Projection
            .Exclude(x => x.Id)
            .Include(x => x.Value)
            .Include(x => x.PartsOfSpeech)
            .Include(x => x.Meanings);

        var word = await wordsCollection
            .Aggregate()
            .Match(wordFilter)
            .Project(wordProjection)
            .Lookup<WordMeaning, LookupResult>(
                foreignCollectionName: WordMeaning.DefaultCollectionName,
                localField: nameof(Word.Meanings),
                foreignField: "_id",
                @as: nameof(LookupResult.MeaningsJoined))
            .FirstOrDefaultAsync();

        return result.With(new GetWordQueryResponse(
            new WordDetailsDTO(
                query.WordId,
                word.Value,
                word.MeaningsJoined.Select(m => new WordMeaningDTO(m.Id.Value, m.Translations.AggregateOrDefault((x, y) => $"{x}, {y}"))).ToArray(),
                word.PartsOfSpeech.Select(m => m.Value).ToArray())));
    }

    public record WordProjection(
        string Value, 
        List<PartOfSpeech> PartsOfSpeech, 
        WordMeaningId[] Meanings,
        WordProperty[]? Properties = null);

    public record LookupResult(
        WordId Id, 
        string Value, 
        List<PartOfSpeech> PartsOfSpeech, 
        WordMeaningId[] Meanings, 
        WordMeaning[] MeaningsJoined);
}
