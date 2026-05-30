namespace ScrutorAttributes.Test.Assembly;

[InjectedSingleton]
public class LibraryClass {}

[InjectedSingleton]
public class InternalLibraryInterfaceImplementation : IInternalLibraryInterface { }

[InjectedSingleton]
public class DoubleInterfaceService : IInterface1, IInterface2 { }

public interface IInternalLibraryInterface { }
public interface ILibraryInterface { }

public interface IInterface1 { }
public interface IInterface2 { }
