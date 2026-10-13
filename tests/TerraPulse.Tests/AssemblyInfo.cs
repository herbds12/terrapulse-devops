// Os testes de integracao compartilham o mesmo banco SQL Server e aplicam
// migrations na inicializacao da API. Desabilitamos o paralelismo entre as
// classes de teste para evitar que duas instancias migrem o banco ao mesmo tempo.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
