from fastapi import FastAPI
from pydantic import BaseModel
from agent.parser import parse_transaction
from agent.julius_agent import JuliusAgent

app = FastAPI()
agent = JuliusAgent()

drafts = []  

class UserMessage(BaseModel):
    message: str

@app.post("/chat")
def chat(msg: UserMessage):
    data = parse_transaction(msg.message)

    if data["amount"]:
        drafts.append(data)
        return {"response": f"Detectei {data}. Confirmar?"}

    return {"response": agent.chat(msg.message)}

@app.get("/drafts")
def get_drafts():
    return drafts
