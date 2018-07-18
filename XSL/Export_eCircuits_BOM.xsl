<?xml version="1.0"?>
<!-- Genera 'partlist' per EURO-CIRCUITS -->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="text" encoding="windows-1252"/>

    <xsl:key 
        name="referenceKey" match="part" use="attributes/attribute[@name='REFERENCE']/@value"/>

    <xsl:template match="/">
        <xsl:text>manufacturer part number, manufacturer, quantity, reference designators</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:apply-templates select="/board/parts"/>
    </xsl:template>

    <xsl:template match="parts">
        <xsl:for-each select="part[generate-id() = generate-id(key('referenceKey', attributes/attribute[@name='REFERENCE']/@value)[1])]">
            <xsl:value-of select="attributes/attribute[@name='REFERENCE']/@value"/>
            <xsl:text>, </xsl:text>
            <xsl:value-of select="attributes/attribute[@name='MANUFACTURER']/@value"/>
            <xsl:text>, </xsl:text>
            <xsl:value-of select="count(key('referenceKey', attributes/attribute[@name='REFERENCE']/@value))" />
            <xsl:text>,</xsl:text>
            <xsl:for-each select="key('referenceKey', attributes/attribute[@name='REFERENCE']/@value)">
                <xsl:text> </xsl:text>
                <xsl:value-of select="@name"/>
            </xsl:for-each>
            <xsl:text>&#13;&#10;</xsl:text>
        </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>