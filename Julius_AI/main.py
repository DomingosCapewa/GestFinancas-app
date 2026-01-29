from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from agent.julius_agent import JuliusAgent
import os

app = FastAPI()

# Configurar CORS - ajustar para produção
allowed_origins = os.getenv("ALLOWED_ORIGINS", "http://localhost:4200,http://localhost:3000").split(",")

app.add_middleware(
    CORSMiddleware,
    allow_origins=allowed_origins,
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
