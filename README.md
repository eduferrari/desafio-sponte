# Desafio back-end Sponte

## **Setup e dados do projeto**

- Framework: ASP .NET Core 3.1 - C# 8
- ORM: Entity Framework Core
- Formato dos dados: JSON
- Dockerização da aplicação para entrega via containers

## **Objetivo**

Seu objetivo nesse desafio será desenvolver um API REST para gerenciamento de matrículas em uma escola de programação.

Para tal, a aplicação precisará suportar os seguintes contextos de negócio:

- Alunos(nome, cpf, foto, data de nascimento, email)
- Cursos(nome, duração em horas, data de limite para matricula, custo, lista de competências ou disciplinas associadas)
- Matricula(data, aluno, lista de cursos, valor total da matrícula)

Como também atender o fluxo padrão de negócio da matrícula, demonstrado abaixo:

![](https://github.com/eduferrari/desafio-sponte/blob/main/RackMultipart20201104-4-16gw9lu_html_4dbdba09d32b0c62.png)
