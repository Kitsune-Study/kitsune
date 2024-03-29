﻿using Manabu.UI.Common.Pages;
using Manabu.UI.Common.Storage;

namespace Manabu.UI.Common.State;

public class WordThemes_IsOriginalShown(IStorage storage) : StateItem<bool>(storage);
public class WordThemes_IsMeaningsShown(IStorage storage) : StateItem<bool>(storage);
public class WordThemes_IsReadingsShown(IStorage storage) : StateItem<bool>(storage);
public class WordThemes_LayoutType(IStorage storage) : StateItem<WordThemesSettings.LayoutType>(storage);
public class MainLayout_IsOriginalMenuShown(IStorage storage) : StateItem<bool>(storage);
