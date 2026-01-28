from sklearn.linear_model import LinearRegression

def predict_spending(history):
    model = LinearRegression()
    X = history[["day"]]
    y = history["amount"]
    model.fit(X, y)
    return model.predict([[31]])
