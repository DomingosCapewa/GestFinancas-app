import re

def parse_transaction(text: str):
    amount = re.findall(r"\d+(?:\.\d+)?", text.replace(",", "."))
    amount = float(amount[0]) if amount else None

    category = "Outros"
    if "comida" in text.lower():
        category = "Alimentação"
    elif "uber" in text.lower() or "transporte" in text.lower():
        category = "Transporte"
    elif "cinema" in text.lower():
        category = "Lazer"

    tx_type = "expense" if "gastei" in text.lower() else "income"

    return {"amount": amount, "category": category, "type": tx_type}
