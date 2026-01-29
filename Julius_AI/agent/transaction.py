import mysql.connector
from datetime import datetime


def save_transaction(user_id, value, category, description):
    conn = mysql.connector.connect(
        host="localhost",
        user="root",
        password="root",
        database="gestfinancas"
    )

    cursor = conn.cursor()

    query = """
    INSERT INTO transactions (user_id, value, category, description, created_at)
    VALUES (%s, %s, %s, %s, %s)
    """

    cursor.execute(query, (user_id, value, category, description, datetime.now()))
    conn.commit()

    cursor.close()
    conn.close()
