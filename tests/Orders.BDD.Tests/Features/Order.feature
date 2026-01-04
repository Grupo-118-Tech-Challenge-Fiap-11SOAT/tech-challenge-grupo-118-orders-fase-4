#language: pt-BR
Funcionalidade: Gerenciamento de Pedidos
    Como um cliente
    Eu quero realizar pedidos
    Para que eu possa receber minha comida

    Cenário: Criar um novo pedido com sucesso
        Dado que eu tenho um pedido válido
        Quando eu envio o pedido
        Então o pedido deve ser criado com sucesso
