from pydantic import BaseModel
from datetime import date
from enum import Enum


class TransactionType(str, Enum):
    income = "Income"
    expense = "Expense"


class TransactionSource(str, Enum):
    manual = "Manual"
    ai = "AI"


class TransactionDraft(BaseModel):
    user_id: str
    amount: float
    description: str
    category: str
    type: TransactionType
    date: date
    source: TransactionSource = TransactionSource.ai
    