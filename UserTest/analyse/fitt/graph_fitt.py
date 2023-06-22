from sklearn.linear_model import LinearRegression
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import math
import sys


def distance_3d(x1, y1, z1, x2, y2, z2):
    distance = math.sqrt((x2 - x1)**2 + (y2 - y1)**2 + (z2 - z1)**2)
    return distance


def compute_distance(df: pd.DataFrame):
    distances = df.apply(lambda row: distance_3d(
        row["MouseX"], row["MouseY"], row["MouseZ"],
        row["nectarX"], row["nectarY"], row["nectarZ"]), axis=1)
    distances.name = "Distance"
    return pd.concat([distances, df["TaskTime"], df["width"], df["GameTime"]], axis=1)


def get_indices(df: pd.DataFrame):
    last = 0
    indices = []
    time = df["GameTime"]
    for d in enumerate(time):
        if d[1] < last:
            indices.append(d[0])
        last = d[1]
    # il faut les bords à chaque fois
    indices.insert(0, 0)
    indices.append(len(time))
    return indices


def get_party_number(index, indicies):
    for ind in indicies:
        if index < ind:
            return indicies.index(ind)


def separate_parties(df: pd.DataFrame):
    indices = get_indices(df)
    df["PartyNumber"] = df.apply(lambda row: get_party_number(row.name, indices), axis=1)
    return df


def compute_fitt(file_name = "fitt.csv"):
    # Créer un DataFrame exemple
    df = pd.read_csv(file_name)

    # # On calcule les distances
    df = compute_distance(df)

    # On obtient les différentes parties
    df = separate_parties(df)

    # df:      Distance   TaskTime      width  GameTime  PartyNumber
    df["IndexOfDifficulty"] = np.log2((df["Distance"] * 2) / df["width"])

    df = df.sort_values("TaskTime")

    X = df[["IndexOfDifficulty"]]
    y = df["TaskTime"]

    regression = LinearRegression()

    # Entraîner le modèle sur les données
    regression.fit(X, y)

    # Prédire les valeurs avec le modèle entraîné
    predictions = regression.predict(X)

    print("predictions:", predictions)

    # Afficher les coefficients et l'intercept du modèle
    print("Coefficients :", regression.coef_)
    print("Intercept :", regression.intercept_)

    plt.scatter(df["IndexOfDifficulty"], df["TaskTime"])
    plt.plot(df["IndexOfDifficulty"], (regression.coef_ * df["IndexOfDifficulty"]) + regression.intercept_)
    plt.xlabel("Index of Difficulty (px^2)/px", fontsize=24)
    plt.ylabel("Index of Performence (sec)", fontsize=24)
    plt.show()


if len(sys.argv) > 1:
    compute_fitt(sys.argv[1])
