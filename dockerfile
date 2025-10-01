FROM python:3.11-slim

# Define o diretório de trabalho
WORKDIR /app

# Copia o arquivo Python para o container
COPY sistema_faculdade.py ./


# Comando para rodar o script
CMD ["python", "sistema_faculdade.py"]
