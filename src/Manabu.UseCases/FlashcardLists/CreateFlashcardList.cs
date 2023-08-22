﻿using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using FluentValidation;
using Manabu.Entities.FlashcardLists;
using Manabu.Entities.Users;
using Mediator;

namespace Manabu.UseCases.FlashcardLists;

public class CreateFlashcardListCommandHandler : ICommandHandler<CreateFlashcardListCommand, Result>
{
    private readonly IRepository<FlashcardList, FlashcardListId> _flashcardListRepository;

    public CreateFlashcardListCommandHandler(IRepository<FlashcardList, FlashcardListId> flashcardListRepository)
    {
        _flashcardListRepository = flashcardListRepository;
    }

    public async ValueTask<Result> Handle(CreateFlashcardListCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        user = new User(userId);

        await _userRepository.Save(user, result);

        return result;
    }
}

public record CreateFlashcardListCommand(
    string UserID) : ICommand<Result>;

public class CreateFlashcardListCommandValidator : AbstractValidator<CreateFlashcardListCommand> {}
