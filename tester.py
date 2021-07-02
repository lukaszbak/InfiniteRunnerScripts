if __name__ == '__main__':
    x = 1
    y = 10
    i = 0
    j = 30
    k = 60
    l = 90
    while x < y :
        if i < j:
            x += ((y-x)/120) 
            print(x)
            i +=1 
        if i >= j and i < k:
            x += ((y-x)/60) 
            print(x)
            i +=1
        if i >= k:
            x += ((y-x)/60) 
            print(x)
            i += 1
    print(x)