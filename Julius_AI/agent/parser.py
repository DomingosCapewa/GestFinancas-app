import re

def parse_transaction(text):
    amount = re.findall(r"\d+(?:,\d+)?", text.replace(",", "."))
    amount = float(amount[0]) if amount else None

    category = "Outros"
    if "comida" in text.lower():
        category = "Alimentação"
    elif "uber" in text.lower():
        category = "Transporte"

    return {
        "amount": amount,
        "category": category,
        "type": "expense" if "gastei" in text.lower() else "income"
    }
