from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from agent.julius_agent import JuliusAgent

app = FastAPI()

# Configurar CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Permitir todos os origins (mudar para URLs específicas em produção)
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

agent = JuliusAgent()

class UserMessage(BaseModel):
    message: str
    user_id: str

@app.post("/chat")
def chat(msg: UserMessage):
    return {"response": agent.chat(msg.message, msg.user_id)}
