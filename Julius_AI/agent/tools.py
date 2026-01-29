from services.dotnet_client import send_draft_transaction

def create_transaction_draft(tx: dict):
    try:
        return send_draft_transaction(tx)
    except Exception as e:
        print(".NET API offline:", e)
        return {"status": "offline"}
