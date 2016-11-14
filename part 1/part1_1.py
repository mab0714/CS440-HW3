# -*- coding: utf-8 -*-
"""
Created on Sun Oct 30 01:20:24 2016

@author: justinliu
"""

import numpy as np
import matplotlib.pyplot as plt
from matplotlib import cm
from plot_confusion_matrix import plot_confusion_matrix
from sklearn.metrics import confusion_matrix
from numpy.random import randn

# In[]:
def read_file(data_file_name, label_file_name):
    """
    """

    if data_file_name == 'trainingimages':
        label_set = np.zeros(5000)
        #data_set = np.chararray([5000, 784])
        image_set = np.chararray([5000, 28, 28])
        with open(label_file_name) as label_file:
            label = label_file.readlines()
        for i in range(5000): 
            label_set[i] = float(label[i][0])
        with open(data_file_name) as data_file:
            data = data_file.readlines()
            for i in range(5000): 
                for x in range(28):
                    for y in range(28):
                        image_set[i][x][y] = data[i*28+x][y]
                        #data_set[i][x*28+y] = data[i*28+x][y]
    else:
        label_set = np.zeros(1000)
        #data_set = np.chararray([5000, 784])
        image_set = np.chararray([1000, 28, 28])
        with open(label_file_name) as label_file:
            label = label_file.readlines()
        for i in range(1000): 
            label_set[i] = float(label[i][0])
        with open(data_file_name) as data_file:
            data = data_file.readlines()
            for i in range(1000): 
                for x in range(28):
                    for y in range(28):
                        image_set[i][x][y] = data[i*28+x][y]
            
    return label_set, image_set #return the data set and label set
    
# In[]:
def train(train_label, train_image):
    """
    """
    
    k = 1
    v = 2
    priors = np.zeros(10) 
    
    # likelihood = [class][foreground, background]
    likelihood = np.zeros([10, 2, 28, 28])
    
    for i in range(5000):
        for x in range(28):
            for y in range(28):
                if train_image[i][x][y] == '#' or train_image[i][x][y] == '+':
                    likelihood[int(train_label[i])][0][x][y] += 1
                else:
                    likelihood[int(train_label[i])][1][x][y] += 1

    # Calculating the P(Fij = f | class)
    for i in range(10):
        temp_sum = likelihood[i][0][0][0] + likelihood[i][1][0][0]
        priors[i] = temp_sum / 5000 # calculating the  P(class)
        for x in range(28):
            for y in range(28):
                likelihood[i][0][x][y] = (likelihood[i][0][x][y] + k) / (temp_sum + k * v)
                likelihood[i][1][x][y] = (likelihood[i][1][x][y] + k) / (temp_sum + k * v)
           
    return likelihood, priors
 
# In[]:   
def test(test_data, likelihood, priors):
    """
    """
    
    posteriors = np.zeros([1000, 10])
    test_likelihood = np.zeros([1000, 10, 28, 28])
    predict_label = np.zeros(1000)
    log_likelihood = np.zeros([10, 2, 28, 28])

    # Match the likelihood according to training results
    log_likelihood = np.log(likelihood)
    for i in range(1000):
        for x in range(28):
            for y in range(28):
                if test_data[i][x][y] == '#' or test_data[i][x][y] == '+':
                    for j in range(10):
                        test_likelihood[i][j][x][y] = log_likelihood[j][0][x][y]
                else:
                    for j in range(10):
                        test_likelihood[i][j][x][y] = log_likelihood[j][1][x][y]
                
    # Calculating the posteriors
    posteriors = np.sum(test_likelihood, axis=3)
    posteriors = np.sum(posteriors, axis=2)

    for i in range(1000):
        posteriors[i] = posteriors[i] + priors
    #posteriors = np.random.rand(1000,10)
    #np.savetxt('posteriors', posteriors)      
    
    # MAP
    for i in range(1000):
        predict_label[i] = np.argmax(posteriors[i])
    predict_label = predict_label.astype(np.int)
    
    return predict_label, test_likelihood
    
# In[]:
def accuracy(test_label, predict_label):
    """
    """

    a = 0
    for i in range(len(predict_label)):
        if test_label[i] == predict_label[i]:
            a += 1
            #acc = a / len(predict_label) * 100

    return a
# In[]:
if __name__ == '__main__':
    
    train_label, train_data = read_file('trainingimages', 'traininglabels')
    test_label, test_data = read_file('testimages', 'testlabels')
    likelihood, priors = train(train_label, train_data)
    predict_label, test_likelihood = test(test_data, likelihood, priors)
    np.savetxt('predict_label', predict_label, fmt='%d')
    accuracy = accuracy(test_label, predict_label)
    accuracy = accuracy / 1000.0
    print accuracy
#    print test_label[1]
#    print predict_label[1]
#    print (test_label[1] == predict_label[1])
    
    # evaluation, drawing the confusion matrix
    c_m = confusion_matrix(test_label, predict_label)
    index = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']
    np.set_printoptions(precision=2)
    plt.figure()
    plot_confusion_matrix(c_m, index, normalize=True, title='Confusion Matrix')
    plt.show()

    # Likelihood and Odds Ratio
    # Make plot with vertical (default) colorbar
    fig1, ax1 = plt.subplots()
    cax1 = ax1.imshow(np.log(likelihood[4][0]), interpolation='nearest', cmap=cm.jet)
    ax1.set_title('Likelihood of 4')
    # Add colorbar, make sure to specify tick locations to match desired ticklabels
    cbar1 = fig1.colorbar(cax1, ticks=[-1, -2, -3, -4, -5])
    cbar1.ax.set_yticklabels(['>-1', '-2', '-3', '-4', '<-5'])  # vertically oriented colorbar
    
    fig2, ax2 = plt.subplots()
    cax2 = ax2.imshow(np.log(likelihood[9][0]), interpolation='nearest', cmap=cm.jet)
    ax2.set_title('Likelihood of 9')
    # Add colorbar, make sure to specify tick locations to match desired ticklabels
    cbar2 = fig2.colorbar(cax2, ticks=[-1, -2, -3, -4, -5])
    cbar2.ax.set_yticklabels(['>-1', '-2', '-3', '-4', '<-5'])  # vertically oriented colorbar
    
    #odds(Fij=1, c1, c2) = P(Fij=1 | c1) / P(Fij=1 | c2).
    odds = np.log(likelihood[1][0] / likelihood[8][0])
    fig3, ax3 = plt.subplots()
    cax3 = ax3.imshow(odds, interpolation='nearest', cmap=cm.jet)
    ax3.set_title('Odds ratio')
    # Add colorbar, make sure to specify tick locations to match desired ticklabels
    cbar3 = fig3.colorbar(cax3, ticks=[2, 1, 0, -1, -2])
    cbar3.ax.set_yticklabels(['>2', '1', '0', '-1', '<-2'])  # vertically oriented colorbar
#    # Make plot with horizontal colorbar
#    fig, ax = plt.subplots()
#    
#    data = np.clip(randn(250, 250), -1, 1)
#    
#    cax = ax.imshow(data, interpolation='nearest', cmap=cm.afmhot)
#    ax.set_title('Gaussian noise with horizontal colorbar')
#    
#    cbar = fig.colorbar(cax, ticks=[-1, 0, 1], orientation='horizontal')
#    cbar.ax.set_xticklabels(['Low', 'Medium', 'High'])  # horizontal colorbar
    plt.show()