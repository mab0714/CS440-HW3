# -*- coding: utf-8 -*-
"""
Created on Mon Nov  7 17:06:37 2016

@author: justinliu
"""

import numpy as np
import time
#import matplotlib.pyplot as plt
#from plot_confusion_matrix import plot_confusion_matrix
#from sklearn.metrics import confusion_matrix
#import itertools as it

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
def train(ori_train_label, ori_train_image, patch_size, square):
    """
    Naive Bayes Training with relaxiation 
    """
    
    k = 1.
    v = 2.
    priors = np.zeros(10)
    xp = patch_size[0]
    yp = patch_size[1]
    train_image = np.zeros([5000, 28, 28])
    
    for i in range(5000):
        for x in range(28):
            for y in range(28):
                if ori_train_image[i][x][y] == '#' or \
                        ori_train_image[i][x][y] == '+':
                    train_image[i][x][y] = 1
                else:
                    train_image[i][x][y] = 0
    
    # likelihood = [class][foreground, background]
#    class_likelihood = {}
#    image_likelihood = [class_likelihood for i in range((28/xp)*(28/yp))]
#    likelihood = [image_likelihood for i in range(10)]

    start = time.clock()
   
    if square == 'disjoint':
        # disjoint
        likelihood = []
        for i in range(10):
            likelihood.append([{} for _ in range(0, (28/xp) * (28/yp))])
    
        for i in range(5000):
            for x in range(28/xp):
                for y in range(28/yp):
                    if tuple(tuple(pixel for pixel in row) \
                            for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp]) in \
                            likelihood[int(train_label[i])][x*(28/yp)+y]:
                        likelihood[int(train_label[i])][x*(28/yp)+y][tuple(tuple(pixel for pixel in row) \
                            for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp])] += 1
                    else:
                        likelihood[int(train_label[i])][x*(28/yp)+y][tuple(tuple(pixel for pixel in row) \
                            for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp])] = 1
        
        # Calculating the P(Fij = f | class)
        likelihood_sum = [0,0,0,0,0,0,0,0,0,0]
        for i in range(10):
            likelihood_sum[i] = np.sum(likelihood[i][0].values())
            for j in range((28/xp)*(28/yp)):
                for key in likelihood[i][j]:
                    likelihood[i][j][key] = (likelihood[i][j][key] + k) / (likelihood_sum[i] + k * (v ** (xp * yp)))
            priors[i] = np.log(likelihood_sum[i] / 5000.) # calculating the  P(class)
            
    else:
        # overlapping
        likelihood = []
        for i in range(10):
            likelihood.append([{} for _ in range(0, (29-xp) * (29-yp))])
    
        for i in range(5000):
            for x in range(29-xp):
                for y in range(29-yp):
                    if tuple(tuple(pixel for pixel in row) \
                            for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp]) \
                            in likelihood[int(train_label[i])][x*(29-yp)+y]:
                        likelihood[int(train_label[i])][x*(29-yp)+y][tuple(tuple(pixel for pixel in row) \
                            for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp])] += 1
                    else:
                        likelihood[int(train_label[i])][x*(29-yp)+y][tuple(tuple(pixel \
                            for pixel in row) for row in train_image[i][xp*x:xp*x+xp, yp*y:yp*y+yp])] = 1
        
        # Calculating the P(Fij = f | class)
        likelihood_sum = [0,0,0,0,0,0,0,0,0,0]
        for i in range(10):
            likelihood_sum[i] = np.sum(likelihood[i][0].values())
            for j in range((29-xp)*(29-yp)):
                for key in likelihood[i][j]:
                    likelihood[i][j][key] = (likelihood[i][j][key] + k) / (likelihood_sum[i] + k * (v ** (xp * yp)))
            priors[i] = np.log(likelihood_sum[i] / 5000.) # calculating the  P(class)
            
    print('Train running time: ', time.clock() - start)
    
    return likelihood, priors, likelihood_sum
 
# In[]:   
def test(ori_test_data, likelihood, priors, likelihood_sum, patch_size, square):
    """
    """
    
    xp = patch_size[0]
    yp = patch_size[1]
    predict_label = np.zeros(1000)
    posteriors = np.zeros([1000, 10])
    
    # Treat both '#' and '+' as '1' and re-define the data images
    test_data = np.zeros([1000, 28, 28])  
    for i in range(1000):
        for x in range(28):
            for y in range(28):
                if ori_test_data[i][x][y] == '#' or ori_test_data[i][x][y] == '+':
                    test_data[i][x][y] = 1
                else:
                    test_data[i][x][y] = 0

    start = time.clock()
    
    if square == 'disjoint':    
        test_likelihood = np.zeros([1000, 10, 28/xp, 28/yp])
      
        for i in range(10):
            for j in range((28/xp)*(28/yp)):
                for k in likelihood[i][j]:
                    likelihood[i][j][k] = np.log(likelihood[i][j][k])
        
        # Match the likelihood according to training results
        for i in range(1000):
            for x in range(28/xp):
                for y in range(28/yp):
                    for each_class in range(10):
                        if tuple(tuple(pixel for pixel in row) \
                                for row in test_data[i][xp*x:xp*x+xp, yp*y:yp*y+yp]) \
                                in likelihood[each_class][x*(28/yp)+y]:
                            test_likelihood[i][each_class][x][y] = \
                                likelihood[each_class][x*(28/yp)+y][tuple(tuple(pixel for pixel in row) \
                                for row in test_data[i][xp*x:xp*x+xp, yp*y:yp*y+yp])]
                        else:
                            test_likelihood[i][each_class][x][y] = 1 / (likelihood_sum[each_class] + 2**(xp*yp))
    else:
        test_likelihood = np.zeros([1000, 10, 29-xp, 29-yp])
      
        for i in range(10):
            for j in range((29-xp)*(29-yp)):
                for k in likelihood[i][j]:
                    likelihood[i][j][k] = np.log(likelihood[i][j][k])
           
        # Match the likelihood according to training results
        for i in range(1000):
            for x in range(29-xp):
                for y in range(29-yp):
                    for each_class in range(10):
                        if tuple(tuple(pixel for pixel in row) \
                                for row in test_data[i][xp*x:xp*x+xp, yp*y:yp*y+yp]) \
                                in likelihood[each_class][x*(29-yp)+y]:
                            test_likelihood[i][each_class][x][y] = \
                                likelihood[each_class][x*(29-yp)+y][tuple(tuple(pixel for pixel in row) \
                                for row in test_data[i][xp*x:xp*x+xp, yp*y:yp*y+yp])]
                        else:
                            test_likelihood[i][each_class][x][y] = 1 / (likelihood_sum[each_class] + 2**(xp*yp))
                
    # Calculating the posteriors
    posteriors = np.sum(test_likelihood, axis=3)
    posteriors = np.sum(posteriors, axis=2)

    for i in range(1000):
        posteriors[i] = posteriors[i] + priors
    
    # MAP
    for i in range(1000):
        predict_label[i] = np.argmax(posteriors[i])
    predict_label = predict_label.astype(np.int)

    print('Test running time: ', time.clock() - start)
    
    return predict_label, posteriors
    
# In[]:
def accuracy(test_label, predict_label):
    """
    """

    a = 0.
    for i in range(len(predict_label)):
        if test_label[i] == predict_label[i]:
            a += 1.
            acc = (a / len(predict_label)) * 100.

    return acc
# In[]:
if __name__ == '__main__':
    
    patch_size=[4, 4]
    square = 'disjoint'
    train_label, train_data = read_file('trainingimages', 'traininglabels')
    test_label, test_data = read_file('testimages', 'testlabels')
    likelihood, priors, likelihood_sum = train(train_label, train_data, patch_size, square)
    predict_label, posteriors = test(test_data, likelihood, priors, likelihood_sum, patch_size, square)
    np.savetxt('predict_disjoint_4*4', predict_label, fmt='%d')
    accuracy = accuracy(test_label, predict_label)
#    accuracy = accuracy / 1000.0
    print 'The accuracy: ', accuracy, '%'
