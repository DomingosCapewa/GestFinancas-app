from fastapi import FastAPI
from pydantic import BaseModel
from agent.julius_agent import JuliusAgent

app = FastAPI()
agent = JuliusAgent()

class UserMessage(BaseModel):
    message: str
    user_id: str

@app.post("/chat")
def chat(msg: UserMessage):
    response = agent.chat(msg.message, msg.user_id)
    text = None
    if hasattr(response, "text"):
        text = response.text
    elif hasattr(response, "content"):
        text = response.content
    else:
        text = str(response)
    return {"response": text}
