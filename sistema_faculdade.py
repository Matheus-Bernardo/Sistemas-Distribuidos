alunos = []
contador_matriculas = {}
logs = []  # armazena os logs das operações


def salvar_logs():
    with open("logs.txt", "w", encoding="utf-8") as f:
        for log in logs:
            f.write(log + "\n")


def gerar_matricula(curso):
    if curso not in contador_matriculas:
        contador_matriculas[curso] = 1
    else:
        contador_matriculas[curso] += 1
    return f"{curso}{contador_matriculas[curso]}"


def cadastrar_aluno():
    nome = input("Digite o nome do aluno: ")
    email = input("Digite o e-mail do aluno: ")
    curso = input("Digite o curso (ex: GES, GEC, GEA): ").upper()

    matricula = gerar_matricula(curso)
    aluno = {
        "nome": nome,
        "email": email,
        "curso": curso,
        "matricula": matricula
    }

    alunos.append(aluno)
    mensagem = f"Aluno {nome} cadastrado com sucesso! Matrícula: {matricula}"
    print(mensagem)
    logs.append(mensagem)


def listar_alunos():
    if not alunos:
        mensagem = "Nenhum aluno cadastrado."
        print(mensagem)
        logs.append(mensagem)
    else:
        print("\n--- Lista de Alunos ---")
        logs.append("--- Lista de Alunos ---")
        for aluno in alunos:
            info = (
                f"Matrícula: {aluno['matricula']} | Nome: {aluno['nome']} | "
                f"E-mail: {aluno['email']} | Curso: {aluno['curso']}"
            )
            print(info)
            logs.append(info)


def atualizar_aluno():
    matricula = input("Digite a matrícula do aluno a ser atualizado: ")
    for aluno in alunos:
        if aluno["matricula"] == matricula:
            print(f"Atualizando aluno {aluno['nome']}...")
            aluno["nome"] = input("Novo nome: ") or aluno["nome"]
            aluno["email"] = input("Novo e-mail: ") or aluno["email"]
            aluno["curso"] = input("Novo curso (ex: GES, GEC): ").upper() or aluno["curso"]
            mensagem = f"Aluno {aluno['matricula']} atualizado com sucesso!"
            print(mensagem)
            logs.append(mensagem)
            return
    mensagem = "Aluno não encontrado."
    print(mensagem)
    logs.append(mensagem)


def remover_aluno():
    matricula = input("Digite a matrícula do aluno a ser removido: ")
    for aluno in alunos:
        if aluno["matricula"] == matricula:
            alunos.remove(aluno)
            mensagem = f"Aluno {aluno['nome']} removido com sucesso!"
            print(mensagem)
            logs.append(mensagem)
            return
    mensagem = "Aluno não encontrado."
    print(mensagem)
    logs.append(mensagem)


def main():
    while True:
        print("\n===== Sistema de Gerenciamento de Alunos =====")
        print("1. Cadastrar Aluno")
        print("2. Listar Alunos")
        print("3. Atualizar Aluno")
        print("4. Remover Aluno")
        print("5. Sair")

        opcao = input("Escolha uma opção: ")

        if opcao == '1':
            cadastrar_aluno()
        elif opcao == '2':
            listar_alunos()
        elif opcao == '3':
            atualizar_aluno()
        elif opcao == '4':
            remover_aluno()
        elif opcao == '5':
            print("Saindo do sistema...")
            salvar_logs()
            print("Logs salvos em logs.txt")
            break
        else:
            print("Opção inválida! Tente novamente.")
            logs.append("Tentativa de opção inválida no menu.")


if __name__ == "__main__":
    main()