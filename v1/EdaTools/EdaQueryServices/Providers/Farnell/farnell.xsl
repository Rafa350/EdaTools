<?xml version="1.0"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" encoding="utf-8"/>    
    
	<xsl:template match="/">
        <xsl:element name="part">
            <xsl:element name="manufacturers">
                <xsl:apply-templates select="/keywordSearchReturn/products"/>
            </xsl:element>
        </xsl:element>
	</xsl:template>
    
    <xsl:template match="products">
        <xsl:element name="manufacturer">
            <xsl:attribute name="name"><xsl:value-of select="brandName"/></xsl:attribute>
            <xsl:attribute name="partNumber"><xsl:value-of select="translatedManufacturerPartNumber"/></xsl:attribute>
            <xsl:attribute name="partName"><xsl:value-of select="displayName"/></xsl:attribute>
            <xsl:element name="attributes">
                <xsl:apply-templates select="attributes"/>
            </xsl:element>
            <xsl:element name="providers">
                <xsl:element name="provider">
                    <xsl:attribute name="name">Farnell</xsl:attribute>
                    <xsl:attribute name="reference"><xsl:value-of select="sku"/></xsl:attribute>
            <xsl:attribute name="updateDate"/>
            <xsl:attribute name="stockQty"><xsl:value-of select="stock/level"/> </xsl:attribute>
            <xsl:attribute name="minOrderQty"><xsl:value-of select="packSize"/> </xsl:attribute>
                    <xsl:element name="prices">
                        <xsl:apply-templates select="prices"/>
                    </xsl:element>
                </xsl:element>
            </xsl:element>
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="prices">
        <xsl:element name="price">
            <xsl:attribute name="break"><xsl:value-of select="to"/></xsl:attribute>
            <xsl:attribute name="price"><xsl:value-of select="cost"/></xsl:attribute>
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="attributes">
        <xsl:element name="attribute">
            <xsl:attribute name="name"><xsl:value-of select="attributeLabel"/></xsl:attribute>
            <xsl:attribute name="unit"><xsl:value-of select="attributeUnit"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="attributeValue"/></xsl:attribute>
        </xsl:element>
    </xsl:template>
    
       
</xsl:stylesheet>
